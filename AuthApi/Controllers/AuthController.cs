using Microsoft.AspNetCore.Mvc;
using AuthApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
       
        private static List<User> _users = new List<User>();
        private static int _nextUserId = 1;

        public AuthController()
        {
            if (!_users.Any())
            {
                _users.Add(new User { Id = _nextUserId++, Username = "testuser", Password = "password123" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User registrationRequest)
        {
            if (string.IsNullOrWhiteSpace(registrationRequest.Username) || string.IsNullOrWhiteSpace(registrationRequest.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            if (_users.Any(u => u.Username == registrationRequest.Username))
            {
                return Conflict(new { message = "Username already exists." });
            }
            var newUser = new User
            {
                Id = _nextUserId++,
                Username = registrationRequest.Username,
                Password = registrationRequest.Password 
            };

            _users.Add(newUser);

            return Ok(new { message = "Registration successful!", user = new { newUser.Id, newUser.Username } });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }
            var user = _users.FirstOrDefault(u => u.Username == loginRequest.Username && u.Password == loginRequest.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }
            return Ok(new { message = "Login successful!", user = new { user.Id, user.Username } });
        }
    }
}