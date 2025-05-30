using AuthApi.Models;
using AuthApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text;
using System.Text.Json; 
using static GlobalModels;

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

        private RequestDetails GetFullRequestDetails<T>(T model)
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

            return new RequestDetails
            {
                Method = requestMethod,
                Body = requestBodyContent
            };
        }

        [HttpPost]
        [Route("register")]
        public async Task<returnModel> Register([FromBody] RegisterModel model)
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

                return new returnModel
                {
                    status = false,
                    error = "Invalid registration data. Please ensure all required fields are provided and correctly formatted. " + errorMessage,
                    result = {}
                    // {
                    //     request_method = fullRequestDetails.Method,
                    //     request_body = fullRequestDetails.Body
                    // },
                    // Ip = clientIp,
                    // Body = fullRequestDetails.Body,
                    // RequestMethod = fullRequestDetails.Method
                };
            }

            var result = await _authService.Register(model);
            var succeeded = result.Succeeded;
            var error = result.Errors;
            if (result.Succeeded)
            {
                return new returnModel
                {
                    status = succeeded,
                    error = "User registered successfully!",
                    
                    // result = new { succeeded = result.Succeeded },
                    // Ip = clientIp,
                    // Body = fullRequestDetails.Body,
                    // RequestMethod = fullRequestDetails.Method
                };
            }
            else
            {
                var errorDetails = result.Errors.Select(e => new { code = e.Code, description = e.Description }).ToList();
                var errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));

                return new returnModel
                {
                    status = false,
                    error = errorMessage,
                    // Ip = clientIp,
                    result = new
                    {
                        succeeded = result.Succeeded,
                        errors = errorDetails
                    },
                    // Body = fullRequestDetails.Body,
                    // RequestMethod = fullRequestDetails.Method
                };
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<returnModel> Login([FromBody] LoginModel model)
        {
            string? clientIp = GetClientIpAddress();
            var fullRequestDetails = GetFullRequestDetails(model);

            Console.WriteLine($"Login Request from IP: {clientIp}");
            Console.WriteLine($"Request Method: {fullRequestDetails.Method ?? "N/A"}");
            Console.WriteLine($"Request Body: {fullRequestDetails.Body ?? "N/A"}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var errormessage = string.Join(" ", errors);

                return new returnModel
                {
                    status = false,
                    error = "Invalid login data. Please ensure all required fields are provided. " + errormessage,
                    result = new
                    {
                        request_method = fullRequestDetails.Method,
                        request_body = fullRequestDetails.Body
                    },
                    // Ip = clientIp,
                    // Body = fullRequestDetails.Body,
                    // RequestMethod = fullRequestDetails.Method
                };
            }

            var (succeeded, token, expires, errorMessage) = await _authService.Login(model);

            if (succeeded)
            {
                return new returnModel
                {
                    status = true,
                    error = "Login successful!",
                    result = new { Token = token, Expires = expires?.ToString("o")},
                    // Ip = clientIp,
                    // Body = fullRequestDetails.Body,
                    // RequestMethod = fullRequestDetails.Method
                };
            }
            else
            {
                return new returnModel
                {
                    status = false,
                    error = errorMessage ?? "Login failed. Please check your credentials.",
                    result = null,
                    // Ip = clientIp, 
                    // Body = fullRequestDetails.Body,
                    // RequestMethod = fullRequestDetails.Method 
                };
            }
        }
    }

   
}