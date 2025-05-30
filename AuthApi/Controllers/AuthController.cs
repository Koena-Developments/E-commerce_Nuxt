using AuthApi.Models;
using AuthApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuth authService) : ControllerBase
    {
        private readonly IAuth _authService = authService;

        // POST: api/Auth/register
        [HttpPost]
        [Route("register")]
        public async Task<Response> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var errorMessage = string.Join(" ", errors);

                return new Response
                {
                    Newstatus = 400,
                    Message = "Invalid registration data. Please ensure all required fields are provided and correctly formatted. " + errorMessage,
                    Result = null 
                };
            }

            var result = await _authService.Register(model);

            if (result.Succeeded)
            {
                return new Response
                {
                    Newstatus = 200,
                    Message = "User registered successfully!",
                    Result = new { succeeded = result.Succeeded } 
                };
            }
            else
            {
                var errorDetails = result.Errors.Select(e => new { code = e.Code, description = e.Description }).ToList();
                var errorMessage = string.Join(" ", result.Errors.Select(e => e.Description)); 

                return new Response
                {
                    Newstatus = 500,
                    Message = errorMessage,
                    Result = new
                    {
                        succeeded = result.Succeeded, 
                        errors = errorDetails
                    }
                };
            }
        }
        // POST: api/Auth/login

        [HttpPost]
        [Route("login")]
        public async Task<Response> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                // var errorMessage = string.Join(" ", errors);

                return new Response
                {
                    Newstatus = 400,
                    Message = "Invalid login data. Please ensure all required fields are provided. " + errors,
                    Result = null 
                };
            }

            var (succeeded, token, expires, errorMessage) = await _authService.Login(model);

            if (succeeded)
            {
                return new Response
                {
                    Newstatus = 200,
                    Message = "Login successful!",
                    Result = new { Token = token, Expires = expires?.ToString("o") }
                };
            }
            else
            {
                return new Response
                {
                    Newstatus = 500,
                    Message = errorMessage ?? "Login failed. Please check your credentials.",
                    Result = null 
                };
            }
        }
    }
}