using Microsoft.EntityFrameworkCore;
using ShopLite.Api.Data;
using ShopLite.Api.DTOs;
using ShopLite.Api.Models;

namespace ShopLite.Api.Services;

public class ProductService(ShopLiteDbContext db) : IProductService
{
    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(string? search = null, CancellationToken ct = default)
    {
        IQueryable<Product> query = db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string pattern = $"%{search.Trim()}%";
            query = query.Where(p => EF.Functions.Like(p.Name, pattern));
        }

        return await query
            .OrderBy(p => p.Id)
            .Select(p => new ProductResponse(p.Id, p.Name, p.Price, p.StockQuantity))
            .ToListAsync(ct);
    }

    public async Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await db.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductResponse(p.Id, p.Name, p.Price, p.StockQuantity))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        var product = new Product
        {
            Name = request.Name.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(ct);

        return new ProductResponse(product.Id, product.Name, product.Price, product.StockQuantity);
    }
}
