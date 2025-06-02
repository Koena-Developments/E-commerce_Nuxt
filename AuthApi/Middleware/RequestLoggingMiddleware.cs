using static AuthApi.Models.GlobalModels;

namespace AuthApi.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

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

            // Read the request body
            string requestBodyContent = await ReadRequestBodyContent(context);

            // Construct our RequestDetails object (adapting your original structure)
            var requestDetails = new RequestDetails
            {
                Method = requestMethod,
                Body = requestBodyContent,
                RequestUrl = $"{requestPath}{requestQueryString}"
                // You could add ClientIpAddress to RequestDetails if desired
            };

            _logger.LogInformation(
                "Incoming Request: IP={ClientIp}, Method={Method}, Path={Path}, Body={Body}",
                clientIp,
                requestDetails.Method,
                requestDetails.RequestUrl,
                requestDetails.Body.Length > 500 ? requestDetails.Body.Substring(0, 500) + "..." : requestDetails.Body
            );

            // --- Capture Response for Logging ---
            // Create a temporary stream to capture the response body
            var originalResponseBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // --- Pass control to the next middleware in the pipeline (or the endpoint) ---
            await _next(context);

            // --- Logic AFTER the request has been processed by the next middleware/endpoint ---

            // Read the captured response body
            context.Response.Body.Seek(0, SeekOrigin.Begin); // Rewind the stream to read from the beginning
            string responseBodyContent = await new StreamReader(context.Response.Body).ReadToEndAsync();

            // Log response details
            _logger.LogInformation(
                "Outgoing Response: Status={StatusCode}, ContentType={ContentType}, Body={Body}",
                context.Response.StatusCode,
                context.Response.ContentType,
                responseBodyContent.Length > 500 ? responseBodyContent.Substring(0, 500) + "..." : responseBodyContent
            );

            // Copy the captured response body back to the original response stream
            context.Response.Body.Seek(0, SeekOrigin.Begin); // Rewind again for copying
            await responseBody.CopyToAsync(originalResponseBodyStream);
            context.Response.Body = originalResponseBodyStream; // Restore original stream
        }

        private string? GetClientIpAddress(HttpContext context)
        {
            return context.Connection?.RemoteIpAddress?.ToString();
        }

        private async Task<string> ReadRequestBodyContent(HttpContext context)
        {
            try
            {
                // Leave the stream open so the next middleware or controller can still read it.
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();

                // Rewind the stream to the beginning for subsequent readers
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