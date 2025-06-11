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

        [HttpPost]
        [Route("register")] 
        public async Task<GlobalModels.returnModel> Register([FromBody] GlobalModels.RegisterModel model)
        {
            GlobalModels.returnModel response; 

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var modelStateErrorMessage = string.Join(" ", errors);

                response = new GlobalModels.returnModel
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
                var serviceResponse = await _authService.Register(model);

                if (serviceResponse.status)
                {
                    response = new GlobalModels.returnModel
                    {
                        result = new { message = "User registered successfully!" },
                        status = true, 
                        error = string.Empty
                    };
                }
                else
                {
                    var errorMessages = serviceResponse.error ?? "Registration failed.";

                    response = new GlobalModels.returnModel
                    {
                        result = new { message = "Registration failed." },
                        status = false, 
                        error = errorMessages
                    };
                }
            }
            return response;
        }

        [HttpPost]
        [Route("login")] 
        public async Task<GlobalModels.returnModel> Login([FromBody] GlobalModels.LoginModel model)
        {
            GlobalModels.returnModel response; 

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var modelStateErrorMessage = string.Join(" ", errors);
                response = new GlobalModels.returnModel
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
                var serviceResponse = await _authService.Login(model);

                if (serviceResponse.status)
                {
                    var loginResult = serviceResponse.result as GlobalModels.LoginReturnModel;

                    response = new GlobalModels.returnModel
                    {
                        result = new
                        {
                            message = loginResult?.Message ?? "Login successful!",
                            token = loginResult?.Token,
                            expires = loginResult?.Expires.ToString("o") 
                        },
                        status = true,
                        error = string.Empty
                    };
                }
                else
                {
                    response = new GlobalModels.returnModel
                    {
                        result = new { message = "Login failed." },
                        status = false,
                        error = serviceResponse.error ?? "Unknown login error."
                    };
                }
            }
            return response;
        }
    }
}
