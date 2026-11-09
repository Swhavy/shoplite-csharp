using ShopLite.Api.DTOs;

namespace ShopLite.Api.Services;

public interface ICheckoutService
{
    Task<OrderResponse> CheckoutAsync(string customerId, CancellationToken ct = default);
}
