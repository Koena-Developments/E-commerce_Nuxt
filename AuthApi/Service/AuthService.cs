using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;
using AuthApi.Models;
using AuthApi.Repository;
using AuthApi.TFTEntities;
using BCrypt.Net;

namespace AuthApi.service
{
    public class AuthService : IAuth
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AuthDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<GlobalModels.returnModel> Register(RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
                return new GlobalModels.returnModel { status = false, error = "Username is required" };
            if (string.IsNullOrWhiteSpace(model.Email))
                return new GlobalModels.returnModel { status = false, error = "Email is required" };
            if (string.IsNullOrWhiteSpace(model.Password))
                return new GlobalModels.returnModel { status = false, error = "Password is required" };

            var usernameExists = await _context.Users.AnyAsync(u => u.Username == model.Username);
            if (usernameExists)
                return new GlobalModels.returnModel { status = false, error = "Username already exists" };

            var emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (emailExists)
                return new GlobalModels.returnModel { status = false, error = "Email already exists" };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = model.Username,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = model.Username
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new GlobalModels.returnModel
            {
                status = true,
                result = new { message = "User registered successfully!" },
                error = string.Empty
            };
        }

        public async Task<GlobalModels.returnModel> Login(LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return new GlobalModels.returnModel { status = false, error = "Email is required" };
            if (string.IsNullOrWhiteSpace(model.Password))
                return new GlobalModels.returnModel { status = false, error = "Password is required" };

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return new GlobalModels.returnModel { status = false, error = "Invalid email or password" };
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var secretKey = _configuration["JWT:Secret"];
            if (string.IsNullOrEmpty(secretKey))
                throw new Exception("JWT:Secret configuration missing");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            double tokenValidityMinutes = _configuration.GetValue<double>("JWT:TokenValidityInMinutes", 1440);
            var tokenExpiration = DateTime.UtcNow.AddMinutes(tokenValidityMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: tokenExpiration,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var loginResult = new GlobalModels.LoginReturnModel
            {
                Message = "Login successful!",
                Token = tokenString,
                Expires = tokenExpiration
            };

            return new GlobalModels.returnModel
            {
                status = true,
                result = loginResult,
                error = string.Empty
            };
        }
    }
}