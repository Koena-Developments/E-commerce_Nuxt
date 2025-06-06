using AuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthApi.TFTEntities;
using Microsoft.EntityFrameworkCore;
using System;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly AuthDbContext _dbContext;

        public UserController(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (String.IsNullOrEmpty(userID))
            {
                return Unauthorized(new GlobalModels.returnModel
                {
                    status = false,
                    error = "User ID claim not found in token."
                });
            }

            if (!long.TryParse(userID, out long userIdLong))
            {
                return BadRequest(new GlobalModels.returnModel
                {
                    status = false,
                    error = "Invalid User Id format in token"
                });
            }

            var userEntity = await _dbContext.Users
                                             .Where(u => u.Id == userIdLong)
                                             .Select(u => new GlobalModels.UserProfileDto
                                             {
                                                 Id = u.Id,
                                                 Username = u.Username,
                                                 Email = u.Email,
                                                 CreatedAt = u.CreatedAt
                                             }).FirstOrDefaultAsync();
            if (userEntity == null)
            {
                return NotFound(new GlobalModels.returnModel
                {
                    status = true,
                    error = "User profile not found in Database."
                });
            }

            return Ok(new GlobalModels.returnModel
            {
                result = userEntity,
                status = true,
                error = string.Empty
            });
        }

        [HttpPut]
        [Route("{id:long}")]
        public async Task<IActionResult> Updateprofile(long id, GlobalModels.UpdateUserProfileDto userprofile)
        {
            var userEntity = await _dbContext.Users.FindAsync(id);
            if (userEntity == null)
            {
                return NotFound();
            }
            userEntity.Email = userprofile.Email;
            userEntity.Username = userprofile.Username;
            userEntity.Password = userprofile.password;
            _dbContext.SaveChanges();

            return Ok(new GlobalModels.UpdateUserProfileDto
            {
                Username = userEntity.Username,
                Email = userEntity.Email
            });
        }



        [HttpDelete]
        // [Route("{id:long}")]
        public async Task<IActionResult> Deleteuser(long id)
        {
            var userEntity = await _dbContext.Users.FindAsync(id);
            if (userEntity == null)
            {
                return NotFound();
            }
            _dbContext.Users.Remove(userEntity);
            _dbContext.SaveChanges();
            return Ok(new GlobalModels.DeleteUserDto
            {
                result = userEntity.Email,
                status = true,
                message = $"Successfully deleted {userEntity.Username} {userEntity.Email}!!"
            });
        }
    }
}