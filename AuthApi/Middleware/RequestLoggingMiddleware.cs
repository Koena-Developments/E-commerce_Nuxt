using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AuthApi.TFTEntities; 
using AuthApi.Models; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.DependencyInjection; 

namespace AuthApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        public const string RequestBodyKey = "RequestBodyContent";

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("MIDDLE WARE!!!!! - Request initiated.");

            // --- Capture Incoming Request Details ---
            context.Request.EnableBuffering();
            string clientIp = GetClientIpAddress(context);
            string requestMethod = context.Request.Method;
            string requestPath = context.Request.Path;
            string requestQueryString = context.Request.QueryString.ToString();
            string requestBodyContent = await ReadRequestBodyContent(context);

            context.Items[RequestBodyKey] = requestBodyContent;

            _logger.LogInformation(
                "Incoming Request: IP={ClientIp}, Method={Method}, Path={Path}, Body={Body}",
                clientIp,
                requestMethod,
                $"{requestPath}{requestQueryString}",
                requestBodyContent.Length > 500 ? requestBodyContent.Substring(0, 500) + "..." : requestBodyContent
            );

            // --- Prepare to Capture Outgoing Response ---
            var originalResponseBodyStream = context.Response.Body;
            using var responseBodyCaptureStream = new MemoryStream();
            context.Response.Body = responseBodyCaptureStream;

            var trafficEntry = new UserTrafficDatum
            {
                ClientIpAddress = clientIp,
                RequestTimeStamp = DateTime.UtcNow,
                RequestUrl = $"{requestPath}{requestQueryString}",
                RequestBody = requestBodyContent,
                ResponseStatusCode = 0, 
                ExceptionType = null,
                ExceptionMessage = null,
                ExceptionDetails = null
            };

            Exception caughtException = null;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                caughtException = ex;
                trafficEntry.ExceptionType = ex.GetType().Name;
                trafficEntry.ExceptionMessage = ex.Message;
                trafficEntry.ExceptionDetails = ex.ToString();
                trafficEntry.ResponseStatusCode = context.Response.StatusCode == 0 ? 500 : context.Response.StatusCode;

                _logger.LogError(ex, "MIDDLE WARE!!!!! - Exception caught in pipeline: {Message}", ex.Message);
                throw;
            }
            finally
            {
                // --- Capture Outgoing Response Details (AFTER _next(context) completes) ---
                responseBodyCaptureStream.Seek(0, SeekOrigin.Begin);
                string responseBodyContent = await new StreamReader(responseBodyCaptureStream).ReadToEndAsync();

                _logger.LogInformation(
                    "Outgoing Response: Status={StatusCode}, ContentType={ContentType}, Body={Body}",
                    context.Response.StatusCode,
                    context.Response.ContentType,
                    responseBodyContent.Length > 500 ? responseBodyContent.Substring(0, 500) + "..." : responseBodyContent
                );

                // --- Update trafficEntry with final response status ---
                if (caughtException == null)
                {
                    trafficEntry.ResponseStatusCode = context.Response.StatusCode;
                }
                else if (trafficEntry.ResponseStatusCode == 0)
                {
                     trafficEntry.ResponseStatusCode = context.Response.StatusCode == 0 ? 500 : context.Response.StatusCode;
                }
                
                try
                {
                    // RESOLVE AUTHDBCONTEXT FROM THE REQUEST'S SERVICE PROVIDER
                    var scopedDbContext = context.RequestServices.GetRequiredService<AuthDbContext>();
                    scopedDbContext.UserTrafficData.Add(trafficEntry);
                    await scopedDbContext.SaveChangesAsync();
                    Console.WriteLine("MIDDLE WARE!!!!! - User traffic entry saved to database successfully.");
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "MIDDLE WARE!!!!! - ERROR: Failed to save user traffic entry to database: {Message}", dbEx.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MIDDLE WARE!!!!! - ERROR: An unexpected error occurred while saving user traffic entry: {Message}", ex.Message);
                }

                // --- Restore Original Response Body ---
                responseBodyCaptureStream.Seek(0, SeekOrigin.Begin);
                await responseBodyCaptureStream.CopyToAsync(originalResponseBodyStream);
                context.Response.Body = originalResponseBodyStream;
            }
        }

        private string? GetClientIpAddress(HttpContext context)
        {
            return context.Connection?.RemoteIpAddress?.ToString();
        }

        private async Task<string> ReadRequestBodyContent(HttpContext context)
        {
            try
            {
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Seek(0, SeekOrigin.Begin);
                return body;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading request body in middleware.");
                return "Error reading request body.";
            }
        }
    }

    public static class MyCustomMiddlewareExtensions
    {
        public static IApplicationBuilder useRequestLoggingMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}