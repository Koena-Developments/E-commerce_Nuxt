using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AuthApi.Models;
using static AuthApi.Models.GlobalModels; 

namespace AuthApi.Repository
{

    public interface IAuth
    {
        Task<(bool Success, string? ErrorMessage)> Register(RegisterModel model);

        Task<(bool Succeeded, string? Token, DateTime? Expires, string? ErrorMessage)> Login(LoginModel model);
    }
    }


