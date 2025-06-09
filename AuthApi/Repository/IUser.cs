using System.Threading.Tasks;
using AuthApi.Models;

namespace AuthApi.Repository
{
    public interface IUser
    {
        Task<GlobalModels.UserProfileDto?> GetUserProfileByIdAsync(long userId);
        Task<(bool Success, string? ErrorMessage)> UpdateUserProfileAsync(long userId, GlobalModels.UpdateUserProfileDto userProfileDto);
        Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(long userId);
    }
}
