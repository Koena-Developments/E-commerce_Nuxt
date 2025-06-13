using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using AuthApi.Models;
using AuthApi.Interface;

namespace AuthApi.Service
{
    public class CheckoutService : ICheckout
    {
        private readonly IConfiguration _configuration;

        public CheckoutService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<GlobalModels.returnModel> CreateCheckoutSession(List<GlobalModels.CartItemDto> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
            {
                return new GlobalModels.returnModel
                {
                    status = false,
                    error = "Cart cannot be empty."
                };
            }

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in cartItems)
            {
                if (item.Quantity <= 0 || item.Price <= 0 || string.IsNullOrEmpty(item.Name))
                {
                    return new GlobalModels.returnModel
                    {
                        status = false,
                        error = $"Invalid item data for product ID {item.ProductId}. Quantity, price, and name must be positive and non-empty."
                    };
                }

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "zar",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name,
                            Description = $"Product ID: {item.ProductId}",
                            Images = item.ImageUrl != null ? new List<string> { item.ImageUrl } : null
                        },
                    },
                    Quantity = item.Quantity,
                });
            }

            var options = new SessionCreateOptions
            {
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{_configuration["App:FrontendUrl"]}/checkout/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{_configuration["App:FrontendUrl"]}/checkout/cancel",
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", Guid.NewGuid().ToString() }
                }
            };

            var service = new SessionService();
            Session session;

            try
            {
                session = await service.CreateAsync(options);
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"Stripe error creating checkout session: {ex.Message}");
                return new GlobalModels.returnModel
                {
                    status = false,
                    error = $"Payment gateway error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Generic error creating checkout session: {ex.Message}");
                return new GlobalModels.returnModel
                {
                    status = false,
                    error = $"An unexpected error occurred: {ex.Message}"
                };
            }

            return new GlobalModels.returnModel
            {
                status = true,
                result = new { checkoutUrl = session.Url },
                error = string.Empty
            };
        }
    }
}