using AuthApi.Delegates;
using static AuthApi.Models.GlobalModels;

namespace AuthApi.Services
{
    public class RequestEventPublisher
    {
        public event RequestLoggedEventHandler? OnRequestLogged;

        public void PublishRequestDetails(RequestDetails details)
        {
            OnRequestLogged?.Invoke(this, details);
        }
    }
}