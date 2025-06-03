using System; // For Console.WriteLine
using System.IO; // For MemoryStream, StreamReader, SeekOrigin
using System.Text; // For Encoding.UTF8
using System.Threading.Tasks; // For Task
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static AuthApi.Models.GlobalModels; // Assuming RequestDetails is here

namespace AuthApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        // Define a constant key for HttpContext.Items to store the request body
        public const string RequestBodyKey = "RequestBodyContent"; // Make this public for access from controllers

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("MIDDLE WARE!!!!!");

            // Enable buffering so the request body stream can be read multiple times (by middleware and then by model binding)
            context.Request.EnableBuffering();

            string clientIp = GetClientIpAddress(context);
            string requestMethod = context.Request.Method;
            string requestPath = context.Request.Path;
            string requestQueryString = context.Request.QueryString.ToString();

            // Read the request body content
            string requestBodyContent = await ReadRequestBodyContent(context);

            // Store the request body content in HttpContext.Items
            // This makes the string content accessible to controllers or other downstream middleware
            context.Items[RequestBodyKey] = requestBodyContent;

            var requestDetails = new RequestDetails
            {
                Method = requestMethod,
                Body = requestBodyContent,
                RequestUrl = $"{requestPath}{requestQueryString}"
            };

            _logger.LogInformation(
                "Incoming Request: IP={ClientIp}, Method={Method}, Path={Path}, Body={Body}",
                clientIp,
                requestDetails.Method,
                requestDetails.RequestUrl,
                requestDetails.Body.Length > 500 ? requestDetails.Body.Substring(0, 500) + "..." : requestDetails.Body
            );

            // --- Capture Response for Logging ---
            // Store the original response body stream
            var originalResponseBodyStream = context.Response.Body;
            // Create a new memory stream to temporarily store the response body
            using var responseBody = new MemoryStream();
            // Assign the new memory stream as the response body
            context.Response.Body = responseBody;

            // Call the next middleware in the pipeline (which will eventually execute the controller action)
            await _next(context);

            // --- Logic AFTER the request has been processed by the next middleware/endpoint ---

            // Rewind the response body stream to the beginning to read its content
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBodyContent = await new StreamReader(context.Response.Body).ReadToEndAsync();

            // Log response details
            _logger.LogInformation(
                "Outgoing Response: Status={StatusCode}, ContentType={ContentType}, Body={Body}",
                context.Response.StatusCode,
                context.Response.ContentType,
                responseBodyContent.Length > 500 ? responseBodyContent.Substring(0, 500) + "..." : responseBodyContent
            );

            // Rewind the response body stream again (important for copying to original)
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            // Copy the content from the temporary memory stream back to the original response stream
            await responseBody.CopyToAsync(originalResponseBodyStream);
            // Restore the original response body stream
            context.Response.Body = originalResponseBodyStream;
        }

        private string? GetClientIpAddress(HttpContext context)
        {
            return context.Connection?.RemoteIpAddress?.ToString();
        }

        private async Task<string> ReadRequestBodyContent(HttpContext context)
        {
            try
            {
                // Leave the stream open: important so that model binding can still read it later.
                // Added Encoding.UTF8, true (detectEncodingFromByteOrderMarks), 1024 (bufferSize), true (leaveOpen) for robustness.
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true);
                var body = await reader.ReadToEndAsync();

                // Rewind the stream to the beginning for subsequent readers (e.g., [FromBody] in controller)
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