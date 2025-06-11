using static AuthApi.Models.GlobalModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthApi.Interface;

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
            _userService = userService;
        }

        // private int? GetCurrentAuthenticatedUserId()
        // {
        //     var userIDClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //     if (int.TryParse(userIDClaim, out int id))
        //     {
        //         return id;
        //     }
        //     return null;
        // }

        // GET USER PROFILE

        [HttpGet("profile/{id:int}")]
        public async Task<returnModel> GetUserProfile(int id) => await _userService.GetUserProfileByIdAsync(id);

        // UPDATE USER PROFILE

        [HttpPut("profile/{id:int}")]
        public async Task<returnModel> UpdateProfile(int id, [FromBody] UpdateUserProfileDto userProfileDto) => await _userService.UpdateUserProfileAsync(id, userProfileDto);

        // DELETE USER PROFILE

        [HttpDelete("profile/{id:int}")]
        public async Task<returnModel> DeleteUser(int id) => await _userService.DeleteUserAsync(id);

    }
}
