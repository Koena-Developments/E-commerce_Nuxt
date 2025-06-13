using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using static AuthApi.Models.GlobalModels;
using AuthApi.TFTEntities;
using AuthApi.Interface;

namespace AuthApi.Service
{
    public class UserService : IUser
    {
        private readonly AuthDbContext _dbContext;

        public UserService(AuthDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<returnModel> GetUserProfileByIdAsync(int userId)
        {
            var userProfile = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserProfileDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (userProfile == null)
            {
                return new returnModel
                {
                    result = new UpdateUserProfileDto(),
                    status = false,
                    error = "User not found."
                };
            }

            return new returnModel
            {
                result = userProfile,
                status = true,
                error = string.Empty
            };
        }

        public async Task<returnModel> UpdateUserProfileAsync(int userId, UpdateUserProfileDto userProfileDto)
        {
            var userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userEntity == null)
            {
                return new returnModel
                {
                    result = userEntity,
                    status = false,
                    error = "User not found."
                };
            }

            try
            {
                if (!string.IsNullOrEmpty(userProfileDto.Username))
                {
                    if (userEntity.Username != userProfileDto.Username)
                    {
                        userEntity.Username = userProfileDto.Username;
                    }
                }
                else if (!string.IsNullOrEmpty(userProfileDto.Password))
                {
                    userEntity.Password = BCrypt.Net.BCrypt.HashPassword(userProfileDto.Password);
                }
            }
            catch (Exception ex)
            {
                return new returnModel
                {
                    result = null,
                    status = false,
                    error = $"An error occurred while updating the user: {ex.Message}"
                };
            }

            userEntity.UpdatedAt = DateTime.UtcNow;
            userEntity.UpdatedBy = userProfileDto.Username;

            try
            {
                await _dbContext.SaveChangesAsync();

                return new returnModel
                {
                    result = new UpdateUserProfileDto
                    {
                        Username = userEntity.Username,
                        Email = userEntity.Email,
                        Password = null
                    },
                    status = true,
                    error = string.Empty
                };
            }
            catch (Exception ex)
            {
                return new returnModel
                {
                    result = null,
                    status = false,
                    error = $"An error occurred while updating the user: {ex.Message}"
                };
            }
        }

        public async Task<returnModel> DeleteUserAsync(int userId)
        {
            var userEntity = await _dbContext.Users.FindAsync(userId);
            if (userEntity == null)
            {
                return new returnModel
                {
                    result = null,
                    status = false,
                    error = "User not found."
                };
            }

            try
            {
                _dbContext.Users.Remove(userEntity);
                await _dbContext.SaveChangesAsync();

                return new returnModel
                {
                    result = new DeleteUserDto
                    {
                        result = new { message = $"Successfully deleted user: {userEntity.Username}" },
                        status = true,
                        message = ""
                    },
                    status = true,
                    error = string.Empty
                };
            }
            catch (Exception ex)
            {
                return new returnModel
                {
                    result = null,
                    status = false,
                    error = $"An error occurred while deleting the user: {ex.Message}"
                };
            }
        }
    }
}