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
                    Console.WriteLine($"Error serializing model to JSON in GetFullRequestDetails: {ex.Message}");
                    requestBodyContent = "Error serializing model.";
                }
            }

            return new AuthApi.Models.GlobalModels.RequestDetails
            {
                Method = requestMethod,
                Body = requestBodyContent,
                RequestUrl = HttpContext?.Request?.Path.ToString()
            };
        }

        private void LogUserTrafficEntry(AuthApi.Models.GlobalModels.UserTrafficEntry entry)
        {
            Console.WriteLine("\n--- User Traffic Entry ---");
            Console.WriteLine($"Client IP: {entry.ClientIpAddress}");
            Console.WriteLine($"Request Time: {entry.RequestTimeStamp}");
            Console.WriteLine($"Response Status Code: {entry.ResponseStatusCode?.ToString() ?? "N/A"}");
            Console.WriteLine($"Request URL: {entry.RequestUrl ?? "N/A"}");
            Console.WriteLine($"Request Body: {entry.RequestBody ?? "N/A"}");
            Console.WriteLine($"Exception Type: {entry.ExceptionType ?? "N/A"}");
            Console.WriteLine($"Exception Message: {entry.ExceptionMessage ?? "N/A"}");
            Console.WriteLine($"Exception Details: {entry.ExceptionDetails ?? "N/A"}");
            Console.WriteLine("--------------------------\n");
        }

        [HttpPost]
        [Route("register")]
        public async Task<AuthApi.Models.GlobalModels.returnModel> Register([FromBody] RegisterModel model)
        {
            var trafficEntry = new AuthApi.Models.GlobalModels.UserTrafficEntry
            {
                ClientIpAddress = GetClientIpAddress(),
                RequestTimeStamp = DateTime.UtcNow,
                RequestBody = GetFullRequestDetails(model).Body,
                RequestUrl = HttpContext?.Request?.Path.ToString(),
            };

            AuthApi.Models.GlobalModels.returnModel response;

            try
            {
                string? clientIp = GetClientIpAddress();
                var fullRequestDetails = GetFullRequestDetails(model);

                Console.WriteLine($"Register Request from IP: {clientIp}");
                Console.WriteLine($"Request Body: {fullRequestDetails.Body ?? "N/A"}");
                Console.WriteLine($"Request Method: {fullRequestDetails.Method ?? "N/A"}");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    var modelStateErrorMessage = string.Join(" ", errors);

                    response = new AuthApi.Models.GlobalModels.returnModel
                    {
                        result = new
                        {
                            message = "Invalid registration data. Please ensure all required fields are provided and correctly formatted."
                        },
                        status = false,
                        error = modelStateErrorMessage
                    };
                    trafficEntry.ResponseStatusCode = 400;
                    trafficEntry.ExceptionType = "ValidationError";
                    trafficEntry.ExceptionMessage = "Model state validation failed.";
                    trafficEntry.ExceptionDetails = modelStateErrorMessage;
                }
                else
                {
                    var result = await _authService.Register(model);

                    if (result.Succeeded)
                    {
                        response = new AuthApi.Models.GlobalModels.returnModel
                        {
                            result = new { message = "User registered successfully!" },
                            status = true,
                            error = string.Empty
                        };
                        trafficEntry.ResponseStatusCode = 200; 
                    }
                    else
                    {
                        var errorMessages = string.Join(" ", result.Errors.Select(e => e.Description));

                        response = new AuthApi.Models.GlobalModels.returnModel
                        {
                            result = new { message = "Registration failed." },
                            status = false,
                            error = errorMessages
                        };

                        trafficEntry.ExceptionType = "RegistrationFailure";
                        trafficEntry.ExceptionMessage = errorMessages;
                        trafficEntry.ExceptionDetails = $"Identity registration failed with errors: {errorMessages}";

                        if (errorMessages.Contains("User already exists", StringComparison.OrdinalIgnoreCase))
                        {
                            trafficEntry.ResponseStatusCode = 409; 
                        }
                        else
                        {
                            trafficEntry.ResponseStatusCode = 500; 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                trafficEntry.ExceptionType = ex.GetType().Name;
                trafficEntry.ExceptionMessage = ex.Message;
                trafficEntry.ExceptionDetails = ex.ToString();

                response = new AuthApi.Models.GlobalModels.returnModel
                {
                    result = new { message = "An unexpected error occurred during registration." },
                    status = false,
                    error = "An internal server error occurred."
                };
                trafficEntry.ResponseStatusCode = 500; 
            }
            finally
            {
                LogUserTrafficEntry(trafficEntry);
            }

            return response;
        }

        [HttpPost]
        [Route("login")]
        public async Task<AuthApi.Models.GlobalModels.returnModel> Login([FromBody] LoginModel model)
        {
            var trafficEntry = new AuthApi.Models.GlobalModels.UserTrafficEntry
            {
                ClientIpAddress = GetClientIpAddress(),
                RequestTimeStamp = DateTime.UtcNow,
                RequestBody = GetFullRequestDetails(model).Body,
                RequestUrl = HttpContext?.Request?.Path.ToString(),
            };

            AuthApi.Models.GlobalModels.returnModel response;

            try
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

                    response = new AuthApi.Models.GlobalModels.returnModel
                    {
                        result = new
                        {
                            message = "Invalid login data. Please ensure all required fields are provided."
                        },
                        status = false,
                        error = modelStateErrorMessage
                    };
                    trafficEntry.ResponseStatusCode = 400; 
                    trafficEntry.ExceptionType = "ValidationError";
                    trafficEntry.ExceptionMessage = "Model state validation failed.";
                    trafficEntry.ExceptionDetails = modelStateErrorMessage;
                }
                else
                {
                    var (succeeded, token, expires, errorMessage) = await _authService.Login(model);

                    if (succeeded)
                    {
                        response = new AuthApi.Models.GlobalModels.returnModel
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
                        trafficEntry.ResponseStatusCode = 200;
                    }
                    else
                    {
                        response = new AuthApi.Models.GlobalModels.returnModel
                        {
                            result = new { message = "Login failed." },
                            status = false,
                            error = errorMessage ?? "Unknown login error."
                        };

                        trafficEntry.ExceptionType = "AuthenticationFailure";
                        trafficEntry.ExceptionMessage = errorMessage ?? "Unknown login error.";
                        trafficEntry.ExceptionDetails = $"Authentication failed: {errorMessage ?? "No specific error provided."}";

                        trafficEntry.ResponseStatusCode = 401;
                    }
                }
            }
            catch (Exception ex)
            {
                trafficEntry.ExceptionType = ex.GetType().Name;
                trafficEntry.ExceptionMessage = ex.Message;
                trafficEntry.ExceptionDetails = ex.ToString(); 

                response = new AuthApi.Models.GlobalModels.returnModel
                {
                    result = new { message = "An unexpected error occurred during login." },
                    status = false,
                    error = "An internal server error occurred."
                };
                trafficEntry.ResponseStatusCode = 500; 
            }
            finally
            {
                LogUserTrafficEntry(trafficEntry);
            }

            return response;
        }
    }
}