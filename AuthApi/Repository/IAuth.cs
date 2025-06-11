using System.Threading.Tasks;
using AuthApi.Models;

namespace AuthApi.Repository
{
    public interface IAuth
    {
        Task<GlobalModels.returnModel> Register(GlobalModels.RegisterModel model);
        Task<GlobalModels.returnModel> Login(GlobalModels.LoginModel model);
    }
}
