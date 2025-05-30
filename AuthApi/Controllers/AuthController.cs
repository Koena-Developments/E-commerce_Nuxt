using AuthApi.Models;
using AuthApi.Repository; 
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Identity; 
using System.Linq;
using System.Threading.Tasks; 

[Route("api/[controller]")] 
[ApiController] 
public class AuthController : ControllerBase
{
    private readonly IAuth _authService; 

    public AuthController(IAuth authService)
    {
        _authService = authService;
    }

    // POST: api/Auth/register
    // Registers a new user
    [HttpPost]
    [Route("register")] 
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Response
            {
                Status = "Error",
                Message = "Invalid registration data. Please ensure all required fields are provided and correctly formatted."
            });
        }

        // Call the Authentication Service to Register the User
        var result = await _authService.Register(model);

        if (result.Succeeded)
        {
            return Ok(new Response
            {
                Status = "Success",
                Message = "User registered successfully!"
            });
        }
        else
        {
           
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));

            return BadRequest(new Response
            {
                Status = "Error",
                Message = errors 
            });

            // return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "An unexpected error occurred during registration: " + errors });
        }
    }

   
    [HttpPost] 
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Response
            {
                Status = "Error",
                Message = "Invalid login data. Please ensure all required fields are provided."
            });
        }

        // Call AuthService to handle login and JWT generation
        var (succeeded, token, expires, errorMessage) = await _authService.Login(model);

        if (succeeded)
        {
            return Ok(new
            {
                Status = "Success",
                Message = "Login successful!",
                Token = token,
                Expires = expires 
            });
        }
        else
        {
            return Unauthorized(new Response
            {
                Status = "Error",
                Message = errorMessage ?? "Login failed. Please check your credentials."
            });
        }
    }
}