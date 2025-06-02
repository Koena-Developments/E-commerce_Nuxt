using AuthApi.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using AuthApi.Models; 

namespace AuthApi.service
{
    public class AuthService : IAuth
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// Registers a new user
        public async Task<IdentityResult> Register(RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Username cannot be empty." });
            }
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Password cannot be empty." });
            }
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email cannot be empty." });
            }


            var userExists = await _userManager.FindByNameAsync(model.Username!);
            if (userExists != null)
            {
                var error = IdentityResult.Failed(new IdentityError
                {
                    Description = "User already exists"
                });
                return error;
            }

            IdentityUser user = new()
            {
                Email = model.Email!,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username! 
            };

            var result = await _userManager.CreateAsync(user, model.Password!);
            return result;
        }

        /// Authenticates a user and generates a JWT token upon successful login.
        public async Task<(bool Succeeded, string? Token, DateTime? Expires, string? ErrorMessage)> Login(LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username)) 
            {
                return (false, null, null, "Username cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return (false, null, null, "Password cannot be empty.");
            }

            var user = await _userManager.FindByNameAsync(model.Username!);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(model.Username!); 
            }


            // Ensure user is not null and password check is successful
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password!)) 
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id ?? throw new InvalidOperationException("User ID is null for a valid user object."))
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole ?? string.Empty));
                }

                var jwtSecret = _configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT:Secret configuration is missing!");
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

                if (!double.TryParse(_configuration["JWT:TokenValidityInHours"], out double tokenValidityHours))
                {
                    tokenValidityHours = 3;
                }
                var tokenExpiryTime = DateTime.UtcNow.AddHours(tokenValidityHours);

                var issuer = _configuration["JWT:ValidIssuer"] ?? throw new InvalidOperationException("JWT:ValidIssuer configuration is missing!");
                var audience = _configuration["JWT:ValidAudience"] ?? throw new InvalidOperationException("JWT:ValidAudience configuration is missing!");

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    expires: tokenExpiryTime,
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return (true, tokenString, tokenExpiryTime, null);
            }

            return (false, null, null, "Invalid username or password.");
        }
    }
}