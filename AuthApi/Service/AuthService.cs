using AuthApi.Models;
using AuthApi.Repository;
using AuthApi.TFTEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<(bool Success, string? ErrorMessage)> Register(RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
                return (false, "Username is required");
            if (string.IsNullOrWhiteSpace(model.Email))
                return (false, "Email is required");
            if (string.IsNullOrWhiteSpace(model.Password))
                return (false, "Password is required");

            var usernameExists = await _context.Users.AnyAsync(u => u.Username == model.Username);
            if (usernameExists)
                return (false, "Username already exists");

            var emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (emailExists)
                return (false, "Email already exists");

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

            return (true, null);
        }

        public async Task<(bool Succeeded, string? Token, DateTime? Expires, string? ErrorMessage)> Login(LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return (Succeeded: false, Token: null, Expires: null, ErrorMessage: "Email is required");
            if (string.IsNullOrWhiteSpace(model.Password))
                return (Succeeded: false, Token: null, Expires: null, ErrorMessage: "Password is required");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            Console.WriteLine("User DATA", user);

            if (user == null)
                return (Succeeded: false, Token: null, Expires: null, ErrorMessage: "Invalid email or password");

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                return (Succeeded: false, Token: null, Expires: null, ErrorMessage: "Invalid email or password");

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

            double tokenValidityHours = 3;
            double.TryParse(_configuration["JWT:TokenValidityInHours"], out tokenValidityHours);

            var tokenExpiration = DateTime.UtcNow.AddHours(tokenValidityHours);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: tokenExpiration,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (Succeeded: true, Token: tokenString, Expires: tokenExpiration, ErrorMessage: null);
        }


    }
}
