using System; 
using System.IO; 
using System.Text;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static AuthApi.Models.GlobalModels;

namespace AuthApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public const string RequestBodyKey = "RequestBodyContent"; 

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("MIDDLE WARE!!!!!");
            context.Request.EnableBuffering();
            string clientIp = GetClientIpAddress(context);
            string requestMethod = context.Request.Method;
            string requestPath = context.Request.Path;
            string requestQueryString = context.Request.QueryString.ToString();

            string requestBodyContent = await ReadRequestBodyContent(context);

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
            var originalResponseBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBodyContent = await new StreamReader(context.Response.Body).ReadToEndAsync();

            _logger.LogInformation(
                "Outgoing Response: Status={StatusCode}, ContentType={ContentType}, Body={Body}",
                context.Response.StatusCode,
                context.Response.ContentType,
                responseBodyContent.Length > 500 ? responseBodyContent.Substring(0, 500) + "..." : responseBodyContent
            );

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalResponseBodyStream);
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