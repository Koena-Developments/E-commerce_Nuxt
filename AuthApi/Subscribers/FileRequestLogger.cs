using AuthApi.Delegates;
using AuthApi.Services;
using static AuthApi.Models.GlobalModels; // To get access to RequestEventPublisher

namespace AuthApi.Subscribers
{
    public class FileRequestLogger
    {
        private readonly ILogger<FileRequestLogger> _logger;

        public FileRequestLogger(RequestEventPublisher publisher, ILogger<FileRequestLogger> logger)
        {
            // Subscribe to the event when this service is created
            publisher.OnRequestLogged += LogRequestToFile;
            _logger = logger;
        }

        // This method matches the RequestLoggedEventHandler delegate signature
        public void LogRequestToFile(object sender, AuthApi.Models.GlobalModels.RequestDetails details)
        {
            // In a real app, you'd write to a file, but for demo, console log
            _logger.LogInformation($"FILE LOG: Request Details: Method={details.Method}, URL={details.RequestUrl}, IP={details.ClientIpAddress}, Body={details.Body}");
            // Example: Append to a log file
            // File.AppendAllText("requests.log", JsonSerializer.Serialize(details) + Environment.NewLine);
        }
    }
}