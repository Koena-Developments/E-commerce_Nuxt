using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using  static AuthApi.Models.GlobalModels;
using AuthApi.Interface;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckout _checkoutService;

        public CheckoutController(ICheckout checkoutService)
        {
            _checkoutService = checkoutService;
        }
        [HttpPost("create-checkout-session")]
        public async Task<returnModel> CreateCheckoutSession([FromBody] List<CartItemDto> cartItems)
        {
            // if (cartItems == null || !cartItems.Any())
            // {
            //     return BadRequest(new GlobalModels.returnModel
            //     {
            //         status = false,
            //         error = "Cart cannot be empty."
            //     });
            // }
                // returnModel serviceResponse = 
            return await _checkoutService.CreateCheckoutSession(cartItems);;
        }
    }
}