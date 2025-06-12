using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AuthApi.Models;
using static AuthApi.Models.GlobalModels;

namespace AuthApi.Interface
{
    public interface IAuth
    {
        Task<returnModel> Register(RegisterModel model);

        Task<returnModel> Login(LoginRequestModel model);
    }

}