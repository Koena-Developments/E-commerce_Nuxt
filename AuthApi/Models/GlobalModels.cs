// File: C:\Users\thaba\Desktop\nuxt_learn\AuthApi\Models\GlobalModels.cs

using System; // Ensure System is included for Object

namespace AuthApi.Models
{
    public class GlobalModels
    {
        public class returnModel
        {
            public Object result { get; set; }

            public bool status { get; set; }

            public string error { get; set; }
        }

        public class RequestDetails
        {
            public string? Method { get; set; }
            public string? Body { get; set; }
        }
    }

    public class RegisterModel
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class LoginModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}