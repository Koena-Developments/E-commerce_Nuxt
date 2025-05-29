using AuthApi;
using AuthApi.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthApi.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

        private readonly IAuth _Auth;
    // private readonly AuthService _service;

    // Constructor to inject UserManager and Configuration
    public AuthController(IAuth auth)
    {
        _Auth = auth;
        // _service = service;
    }

    // POST: api/auth/register
    // Registers a new user
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = await _Auth.Register(model);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

    // POST: api/auth/login
    // Authenticates a user and returns a JWT token
    // [HttpPost]
    // [Route("login")]
    // public async Task<IActionResult> Login([FromBody] LoginModel model)
    // {
        
    // }
}