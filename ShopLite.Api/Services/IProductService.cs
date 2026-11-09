using ShopLite.Api.DTOs;

namespace ShopLite.Api.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> GetAllAsync(string? search = null, CancellationToken ct = default);
    Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
}
