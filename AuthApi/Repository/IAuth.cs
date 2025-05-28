
namespace AuthApi.Repository
{

    public interface IAuth
    {
        Task<IActionResult> Register([FromBody] RegisterModel model);
        Task<IActionResult> Login([FromBody] LoginModel model);
    }

}
