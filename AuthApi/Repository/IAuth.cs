using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Repository
{

    public interface IAuth
    {
        Task<IdentityResult> Register(RegisterModel model);

        Task<(bool Succeeded, string? Token, DateTime? Expires, string? ErrorMessage)> Login(LoginModel model);
    }
    }


