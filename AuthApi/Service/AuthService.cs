using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims; 
using System.Collections.Generic;

using static AuthApi.Models.GlobalModels;
using AuthApi.Interface; 
using AuthApi.TFTEntities;

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
        // Registration Service
        public async Task<returnModel> Register(RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
                return new returnModel { status = false, error = "Username is required", result = null };

            if (string.IsNullOrWhiteSpace(model.Email))
                return new returnModel { status = false, error = "Email is required", result = null };

            if (string.IsNullOrWhiteSpace(model.Password))
                return new returnModel { status = false, error = "Password is required", result = null };

            var usernameExists = await _context.Users.AnyAsync(u => u.Username == model.Username);
            if (usernameExists)
                return new returnModel { status = false, error = "Username already exists", result = null };

            var emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (emailExists)
                return new returnModel { status = false, error = "Email already exists", result = null };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = hashedPassword,
                CreatedAt = DateTime.Now,
                CreatedBy = model.Username,
                UpdatedAt = DateTime.Now,
                UpdatedBy = model.Username
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new returnModel { status = true, error = "", result = user };
        }
        // Login Service
        public async Task<returnModel> Login(LoginRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return new returnModel { status = false, error = "Email is required.", result = null };

            if (string.IsNullOrWhiteSpace(model.Password))
                return new returnModel { status = false, error = "Password is required.", result = null };

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return new returnModel { status = false, error = "Invalid email or password.", result = null };
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var secretKey = _configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT:Secret configuration missing!");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            double tokenValidityMinutes = _configuration.GetValue<double>("JWT:TokenValidityInMinutes", 1440);
            var tokenExpiration = DateTime.UtcNow.AddMinutes(tokenValidityMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"] ?? throw new InvalidOperationException("JWT:ValidIssuer configuration missing!"),
                audience: _configuration["JWT:ValidAudience"] ?? throw new InvalidOperationException("JWT:ValidAudience configuration missing!"),
                claims: claims,
                expires: tokenExpiration,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var loginResponse = new LoginResponseModel
            {
                Token = tokenString,
                Expires = tokenExpiration.ToString("o"),
                Email = user.Email,
                Username = user.Username
            };

            return new returnModel
            {
                result = loginResponse,
                status = true,
                error = string.Empty
            };
        }
    }
}
