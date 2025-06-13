using System.Collections.Generic;
using System.Threading.Tasks;
using AuthApi.Models;

namespace AuthApi.Interface
{
    public interface ICheckout
    {
        Task<GlobalModels.returnModel> CreateCheckoutSession(List<GlobalModels.CartItemDto> cartItems);
    }
}
