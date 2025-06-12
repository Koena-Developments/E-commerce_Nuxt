using System.Threading.Tasks;
using static AuthApi.Models.GlobalModels;

namespace AuthApi.Interface
{
    public interface IUser
    {
        Task<returnModel?> GetUserProfileByIdAsync(int userId);
        Task<returnModel> UpdateUserProfileAsync(int userId, UpdateUserProfileDto userProfileDto);
        Task<returnModel> DeleteUserAsync(int userId);
    }
}