using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Repository
{

    public interface IAuth
    {
        Task<IdentityResult> Register(RegisterModel model);
        // Task<IActionResult> Login([FromBody] LoginModel model);
    }

}
