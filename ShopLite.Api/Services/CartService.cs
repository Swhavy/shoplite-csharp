using Microsoft.EntityFrameworkCore;
using ShopLite.Api.Data;
using ShopLite.Api.DTOs;
using ShopLite.Api.Models;

namespace ShopLite.Api.Services;

public class CartService(ShopLiteDbContext db) : ICartService
{
    public async Task<CartResponse> GetCartAsync(string customerId, CancellationToken ct = default)
    {
        var items = await db.CartItems
            .AsNoTracking()
            .Include(c => c.Product)
            .Where(c => c.CustomerId == customerId)
            .OrderBy(c => c.Id)
            .Select(c => new CartItemResponse(
                c.ProductId,
                c.Product!.Name,
                c.Product.Price,
                c.Quantity,
                c.Product.Price * c.Quantity))
            .ToListAsync(ct);

        decimal total = items.Sum(i => i.Subtotal);
        return new CartResponse(customerId, items, total);
    }

    public async Task<CartResponse> AddItemAsync(string customerId, AddCartItemRequest request, CancellationToken ct = default)
    {
        if (request.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be at least 1.");
        }

        var product = await db.Products.FindAsync([request.ProductId], ct)
            ?? throw new KeyNotFoundException($"Product with ID {request.ProductId} was not found.");

        var existingItem = await db.CartItems
            .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == request.ProductId, ct);

        int newQuantity = (existingItem?.Quantity ?? 0) + request.Quantity;

        if (newQuantity > product.StockQuantity)
        {
            throw new InvalidOperationException(
                $"Insufficient stock for '{product.Name}'. Requested total: {newQuantity}, available stock: {product.StockQuantity}.");
        }

        if (existingItem is null)
        {
            db.CartItems.Add(new CartItem
            {
                CustomerId = customerId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });
        }
        else
        {
            existingItem.Quantity = newQuantity;
        }

        await db.SaveChangesAsync(ct);
        return await GetCartAsync(customerId, ct);
    }
}
