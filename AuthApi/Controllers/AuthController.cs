using AuthApi.Models;
using AuthApi.Interface;
using Microsoft.AspNetCore.Mvc;
using AuthApi.TFTEntities;
using static AuthApi.Models.GlobalModels; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; 

namespace AuthApi.Controllers
{
    [Route("api/[controller]")] 
    [ApiController] 
    [AllowAnonymous] 
    public class AuthController(IAuth authService) : ControllerBase
    {
        private readonly IAuth _authService = authService;
        // private readonly AuthDbContext _authDbContext = authDbContext;

        // --- REGISTER ENDPOINT ---
        [HttpPost]
        [Route("register")]
        public async Task<returnModel> Register([FromBody] RegisterModel model) =>await _authService.Register(model);

        // --- LOGIN ENDPOINT ---
        [HttpPost]
        [Route("login")] 
        public async Task<returnModel> Login([FromBody] LoginRequestModel model) =>  await _authService.Login(model);
      
    }
}