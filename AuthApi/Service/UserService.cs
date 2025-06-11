using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using AuthApi.Models;
using AuthApi.TFTEntities;
using AuthApi.Repository;

namespace AuthApi.service
{
    public class UserService : IUser
    {
        private readonly AuthDbContext _dbContext;

        public UserService(AuthDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<GlobalModels.UserProfileDto?> GetUserProfileByIdAsync(long userId)
        {
            var userEntity = await _dbContext.Users
                                             .Where(u => u.Id == userId)
                                             .Select(u => new GlobalModels.UserProfileDto
                                             {
                                                 Id = u.Id,
                                                 Username = u.Username,
                                                 Email = u.Email,
                                                 CreatedAt = u.CreatedAt
                                             })
                                             .FirstOrDefaultAsync();
            return userEntity;
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateUserProfileAsync(long userId, GlobalModels.UpdateUserProfileDto userProfileDto)
        {
           var userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userEntity == null)
            {
                return (false, "User not found.");
            }

            if (!string.IsNullOrEmpty(userProfileDto.Username) && userEntity.Username != userProfileDto.Username)
            {
                userEntity.Username = userProfileDto.Username;
            }

            if (!string.IsNullOrEmpty(userProfileDto.Password))
            {
                userEntity.Password = BCrypt.Net.BCrypt.HashPassword(userProfileDto.Password);
            }

            userEntity.UpdatedAt = DateTime.UtcNow;
            userEntity.UpdatedBy = userProfileDto.Username;

            try
            {
                await _dbContext.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateConcurrencyException)
            {
                return (false, "Concurrency conflict: User data changed while updating. Please try again.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error updating user profile for ID {userId}: {ex.Message}");
                return (false, "An error occurred while saving changes.");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(long userId)
        {
            var userEntity = await _dbContext.Users.FindAsync(userId);
            if (userEntity == null)
            {
                return (false, "User not found.");
            }

            try
            {
                _dbContext.Users.Remove(userEntity);
                await _dbContext.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error deleting user for ID {userId}: {ex.Message}");
                return (false, "An error occurred while deleting the user.");
            }
        }
    }
}
