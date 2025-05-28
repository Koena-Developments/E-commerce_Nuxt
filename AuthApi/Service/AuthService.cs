using AuthApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace AuthApi.service
{
    public class AuthService : IAuth
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> Register(RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
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
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            return result;
        }


        // public async Task<IActionResult> Login([FromBody] LoginModel model)
        //     {
        //     // Find user by username
        //     var user = await _userManager.FindByNameAsync(model.Username);
        //     // Check if user exists and password is correct
        //     if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        //     {
        //         // Get user roles
        //         var userRoles = await _userManager.GetRolesAsync(user);

        //         // Create claims for JWT token
        //         var authClaims = new List<Claim>
        //         {
        //             new Claim(ClaimTypes.Name, user.UserName),
        //             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //         };

        //         // Add role claims
        //         foreach (var userRole in userRoles)
        //         {
        //             authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        //         }

        //         // Create signing key from configuration
        //         var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        //         // Generate JWT token
        //         var token = new JwtSecurityToken(
        //             issuer: _configuration["JWT:ValidIssuer"],
        //             audience: _configuration["JWT:ValidAudience"],
        //             expires: DateTime.Now.AddHours(3),
        //             claims: authClaims,
        //             signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //         );

        //         // Return token and expiration
        //         return Ok(new
        //         {
        //             token = new JwtSecurityTokenHandler().WriteToken(token),
        //             expiration = token.ValidTo
        //         });
        //     }
        //     // Unauthorized if login fails
        //     return Unauthorized();
        // }
        //     }


    }
}