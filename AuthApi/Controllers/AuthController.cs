using AuthApi.Models;
using AuthApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq; 
using System.Threading.Tasks;
using System;
using AuthApi.Data;
using AuthApi.TFTEntities;
using Microsoft.EntityFrameworkCore;
using static AuthApi.Models.GlobalModels; 

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuth authService, AuthDbContext authDbContext) : ControllerBase
    {
        private readonly IAuth _authService = authService;
        private readonly AuthDbContext _authDbContext = authDbContext;

        private string? GetClientIpAddress()
        {
            return HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        [HttpPost]
        [Route("register")]
        public async Task<returnModel> Register([FromBody] RegisterModel model)
        {
            // The middleware now handles logging the request body, IP, URL, etc.
            // You only need to populate the UserTrafficDatum with response details
            // and any exceptions that occur within the controller's business logic.

            var trafficEntry = new UserTrafficDatum
            {
                ClientIpAddress = GetClientIpAddress(), 
                RequestTimeStamp = DateTime.UtcNow,
                RequestUrl = HttpContext?.Request?.Path.ToString(),
                RequestBody = HttpContext?.Request?.Body.ToString(),
                // RequestBody, RequestMethod are now logged by the middleware
                // and should ideally be stored in the UserTrafficDatum by the middleware
                // if you want them persisted in the DB as well.
                // For this example, we're assuming the middleware takes care of the full request log.
                // If you *still* need it here for the DB entry, you'd need to re-read it,
                // but the middleware has already read it, so it's less efficient.
                // Consider adding a mechanism in your middleware to pass some of these details
                // to the controller if needed for the DB, or have the middleware directly
                // save the UserTrafficDatum.
                ExceptionType = null,
                ExceptionMessage = null,
                ExceptionDetails = null,
                ResponseStatusCode = 0
            };

            returnModel response;

            try
            {
                // Removed redundant Console.WriteLines and GetFullRequestDetails calls
                // as the middleware now handles this for logging.

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
                    trafficEntry.ResponseStatusCode = 400;
                    trafficEntry.ExceptionType = "ValidationError";
                    trafficEntry.ExceptionMessage = "Model state validation failed.";
                    trafficEntry.ExceptionDetails = modelStateErrorMessage;
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
                        trafficEntry.ResponseStatusCode = 200;
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

                response = new returnModel
                {
                    result = new { message = "An unexpected error occurred during registration." },
                    status = false,
                    error = "An internal server error occurred."
                };
                trafficEntry.ResponseStatusCode = 500;
            }
            finally
            {
                // This `finally` block is still crucial for saving the traffic entry to the DB.
                try
                {
                    _authDbContext.UserTrafficData.Add(trafficEntry);
                    await _authDbContext.SaveChangesAsync();
                    Console.WriteLine("User traffic entry saved to database successfully.");
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"ERROR: Failed to save user traffic entry to database: {dbEx.Message}");
                    Console.WriteLine($"Details: {dbEx.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: An unexpected error occurred while saving user traffic entry: {ex.Message}");
                }
            }

            return response;
        }

        [HttpPost]
        [Route("login")]
        public async Task<returnModel> Login([FromBody] LoginModel model)
        {
            var trafficEntry = new UserTrafficDatum
            {
                ClientIpAddress = GetClientIpAddress(),
                RequestTimeStamp = DateTime.UtcNow,
                RequestUrl = HttpContext?.Request?.Path.ToString(),
                // Similar to Register, the middleware handles detailed request logging.
                // Populate exception and status code here for the DB.
                ExceptionType = null,
                ExceptionMessage = null,
                ExceptionDetails = null,
                ResponseStatusCode = 0
            };

            returnModel response;

            try
            {
                // Removed redundant Console.WriteLines and GetFullRequestDetails calls.

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
                        trafficEntry.ResponseStatusCode = 200;
                    }
                    else
                    {
                        response = new returnModel
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

                response = new returnModel
                {
                    result = new { message = "An unexpected error occurred during login." },
                    status = false,
                    error = "An internal server error occurred."
                };
                trafficEntry.ResponseStatusCode = 500;
            }
            finally
            {
                try
                {
                    _authDbContext.UserTrafficData.Add(trafficEntry);
                    await _authDbContext.SaveChangesAsync();
                    Console.WriteLine("User traffic entry saved to database successfully.");
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"ERROR: Failed to save user traffic entry to database: {dbEx.Message}");
                    Console.WriteLine($"Details: {dbEx.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: An unexpected error occurred while saving user traffic entry: {ex.Message}");
                }
            }

            return response;
        }
    }
}