using ShopLite.Api.DTOs;

namespace ShopLite.Api.Services;

public interface ICartService
{
    Task<CartResponse> GetCartAsync(string customerId, CancellationToken ct = default);
    Task<CartResponse> AddItemAsync(string customerId, AddCartItemRequest request, CancellationToken ct = default);
}
