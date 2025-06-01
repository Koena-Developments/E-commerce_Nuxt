// File: C:\Users\thaba\Desktop\nuxt_learn\AuthApi\Controllers\AuthController.cs

using AuthApi.Models;
using AuthApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuth authService) : ControllerBase
    {
        private readonly IAuth _authService = authService;

        private string? GetClientIpAddress()
        {
            return HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        private AuthApi.Models.GlobalModels.RequestDetails GetFullRequestDetails<T>(T model)
        {
            string requestMethod = HttpContext?.Request?.Method ?? "UNKNOWN";
            string requestBodyContent = string.Empty;

            if (model != null)
            {
                try
                {
                    requestBodyContent = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = false });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error serializing model to JSON: {ex.Message}");
                    requestBodyContent = "Error serializing model.";
                }
            }

            return new AuthApi.Models.GlobalModels.RequestDetails
            {
                Method = requestMethod,
                Body = requestBodyContent
            };
        }

        [HttpPost]
        [Route("register")]
        public async Task<AuthApi.Models.GlobalModels.returnModel> Register([FromBody] RegisterModel model)
        {
            string? clientIp = GetClientIpAddress();
            var fullRequestDetails = GetFullRequestDetails(model);

            Console.WriteLine($"Register Request from IP: {clientIp}");
            Console.WriteLine($"Request Body: {fullRequestDetails.Body ?? "N/A"}");
            Console.WriteLine($"Request Method: {fullRequestDetails.Method ?? "N/A"}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var errorMessage = string.Join(" ", errors);

                return new AuthApi.Models.GlobalModels.returnModel
                {
                    result = new
                    {
                        message = "Invalid registration data. Please ensure all required fields are provided and correctly formatted."
                    },
                    status = false,
                    error = errorMessage
                };
            }

            var result = await _authService.Register(model);

            if (result.Succeeded)
            {
                return new AuthApi.Models.GlobalModels.returnModel
                {
                    result = new { message = "User registered successfully!" },
                    status = true,
                    error = string.Empty
                };
            }
            else
            {
                var errorMessages = string.Join(" ", result.Errors.Select(e => e.Description));

                return new AuthApi.Models.GlobalModels.returnModel
                {
                    result = new { message = "Registration failed." },
                    status = false,
                    error = errorMessages
                };
            }
        }

 

        [HttpPost]
        [Route("login")]
        public async Task<AuthApi.Models.GlobalModels.returnModel> Login([FromBody] LoginModel model)
        {
            string? clientIp = GetClientIpAddress();
            var fullRequestDetails = GetFullRequestDetails(model);

            Console.WriteLine($"Login Request from IP: {clientIp}");
            Console.WriteLine($"Request Method: {fullRequestDetails.Method ?? "N/A"}");
            Console.WriteLine($"Request Body: {fullRequestDetails.Body ?? "N/A"}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var modelStateErrorMessage = string.Join(" ", errors);

                return new AuthApi.Models.GlobalModels.returnModel
                {
                    result = new
                    {
                        message = "Invalid login data. Please ensure all required fields are provided."
                    },
                    status = false,
                    error = modelStateErrorMessage
                };
            }

            var (succeeded, token, expires, errorMessage) = await _authService.Login(model);

            if (succeeded)
            {
                return new AuthApi.Models.GlobalModels.returnModel
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
                return new AuthApi.Models.GlobalModels.returnModel
                {
                    result = new { message = "Login failed." },
                    status = false,
                    error = errorMessage ?? "Unknown login error."
                };
            }
        } 
    }
} 