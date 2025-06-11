using static AuthApi.Models.GlobalModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthApi.Interface;
using System.Security.Claims;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(IUser userService) : ControllerBase
    {
        private readonly IUser _userService = userService;

        private int? GetCurrentAuthenticatedUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int id))
            {
            return id;
            }
            return null;
        }

        // GET USER PROFILE
        [HttpGet("profile")]
        public async Task<returnModel?> GetUserProfile() => await _userService.GetUserProfileByIdAsync(GetCurrentAuthenticatedUserId().Value);
        
        // UPDATE USER PROFILE
        [HttpPut("profile")]
        public async Task<returnModel?> UpdateProfile([FromBody] UpdateUserProfileDto userProfileDto)=> await _userService.UpdateUserProfileAsync(GetCurrentAuthenticatedUserId().Value, userProfileDto);


        // DELETE USER PROFILE
        [HttpDelete("profile")]
        public async Task<returnModel?> DeleteUser() => await _userService.DeleteUserAsync(GetCurrentAuthenticatedUserId().Value);
        
    }
}
