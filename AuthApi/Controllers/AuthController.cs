using AuthApi.Models;
using AuthApi.Repository;
using Microsoft.AspNetCore.Mvc;
using AuthApi.TFTEntities;
using static AuthApi.Models.GlobalModels; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; 

namespace AuthApi.Controllers
{
    [Route("api/[controller]")] 
    [ApiController] 
    [AllowAnonymous] 
    public class AuthController(IAuth authService, AuthDbContext authDbContext) : ControllerBase
    {
        private readonly IAuth _authService = authService;
        private readonly AuthDbContext _authDbContext = authDbContext; 


        // --- REGISTER ENDPOINT ---
        [HttpPost]
        [Route("register")] 
        public async Task<returnModel> Register([FromBody] RegisterModel model)
        {
            returnModel response; 

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var modelStateErrorMessage = string.Join(" ", errors);

                response = new returnModel
                {
                    result = new
                    {
                        message = "Invalid registration data. Please ensure all required fields are provided and correctly formatted."
                    },
                    status = false, 
                    error = modelStateErrorMessage
                };
            }
            else
            {
                var result = await _authService.Register(model);

                if (result.Success)
                {
                    response = new returnModel
                    {
                        result = new { message = "User registered successfully!" },
                        status = true, 
                        error = string.Empty
                    };
                }
                else
                {
                    var errorMessages = result.ErrorMessage ?? "Registration failed.";

                    response = new returnModel
                    {
                        result = new { message = "Registration failed." },
                        status = false, 
                        error = errorMessages
                    };
                }
            }

            return response;
        }

        // --- LOGIN ENDPOINT ---
        [HttpPost]
        [Route("login")] 
        public async Task<returnModel> Login([FromBody] LoginModel model)
        {

            returnModel response; 

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var modelStateErrorMessage = string.Join(" ", errors);
                response = new returnModel
                {
                    result = new
                    {
                        message = "Invalid login data. Please ensure all required fields are provided."
                    },
                    status = false,
                    error = modelStateErrorMessage
                };
            }
            else
            {
                var (succeeded, token, expires, errorMessage) = await _authService.Login(model);

                if (succeeded)
                {
                    response = new returnModel
                    {
                        result = new
                        {
                            message = "Login successful!",
                            token = token,
                            expires = expires?.ToString("o") 
                        },
                        status = true,
                        error = string.Empty
                    };
                }
                else
                {
                    response = new returnModel
                    {
                        result = new { message = "Login failed." },
                        status = false,
                        error = errorMessage ?? "Unknown login error."
                    };
                }
            }
            return response;
        }
    }
}