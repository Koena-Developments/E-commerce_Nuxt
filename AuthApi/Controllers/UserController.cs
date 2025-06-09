using AuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

using AuthApi.service;
using AuthApi.Repository;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUser _userService;

        public UserController(IUser userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        private long? GetCurrentAuthenticatedUserId()
        {
            var userIDClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (long.TryParse(userIDClaim, out long userIdLong))
            {
                return userIdLong;
            }
            return null;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = GetCurrentAuthenticatedUserId();

            if (!userId.HasValue)
            {
                return Unauthorized(new GlobalModels.returnModel
                {
                    status = false,
                    error = "User ID claim not found or invalid in token."
                });
            }

            var userProfile = await _userService.GetUserProfileByIdAsync(userId.Value);

            if (userProfile == null)
            {
                return NotFound(new GlobalModels.returnModel
                {
                    status = false,
                    error = "User profile not found."
                });
            }

            return Ok(new GlobalModels.returnModel
            {
                result = userProfile,
                status = true,
                error = string.Empty
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] GlobalModels.UpdateUserProfileDto userProfileDto)
        {
            var userId = GetCurrentAuthenticatedUserId();

            if (!userId.HasValue)
            {
                return Unauthorized(new GlobalModels.returnModel
                {
                    status = false,
                    error = "Invalid or missing user authentication."
                });
            }

            var (success, errorMessage) = await _userService.UpdateUserProfileAsync(userId.Value, userProfileDto);

            if (success)
            {
                var updatedProfile = await _userService.GetUserProfileByIdAsync(userId.Value);
                return Ok(new GlobalModels.returnModel
                {
                    result = updatedProfile,
                    status = true,
                    error = string.Empty
                });
            }
            else
            {
                if (errorMessage == "User not found.")
                {
                    return NotFound(new GlobalModels.returnModel { status = false, error = errorMessage });
                }
                return BadRequest(new GlobalModels.returnModel { status = false, error = errorMessage });
            }
        }

        [HttpDelete("profile")]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = GetCurrentAuthenticatedUserId();

            if (!userId.HasValue)
            {
                return Unauthorized(new GlobalModels.returnModel
                {
                    status = false,
                    error = "Invalid or missing user authentication."
                });
            }

            var (success, errorMessage) = await _userService.DeleteUserAsync(userId.Value);

            if (success)
            {
                return Ok(new GlobalModels.returnModel
                {
                    status = true,
                    result = new {  message = "User deleted successfully."},
                    error = string.Empty,
                });
            }
            else
            {
                if (errorMessage == "User not found.")
                {
                    return NotFound(new GlobalModels.returnModel { status = false, error = errorMessage });
                }
                return BadRequest(new GlobalModels.returnModel { status = false, error = errorMessage });
            }
        }
    }
}
