using Microsoft.EntityFrameworkCore;
using ShopLite.Api.Data;
using ShopLite.Api.DTOs;
using ShopLite.Api.Models;

namespace ShopLite.Api.Services;

public class CheckoutService(ShopLiteDbContext db) : ICheckoutService
{
    public async Task<OrderResponse> CheckoutAsync(string customerId, CancellationToken ct = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var cartItems = await db.CartItems
            .Include(c => c.Product)
            .Where(c => c.CustomerId == customerId)
            .ToListAsync(ct);

        if (cartItems.Count == 0)
        {
            throw new InvalidOperationException($"Cart for customer '{customerId}' is empty.");
        }

        var order = new Order
        {
            CustomerId = customerId,
            CreatedAtUtc = DateTime.UtcNow
        };

        foreach (var item in cartItems)
        {
            var product = item.Product
                ?? throw new InvalidOperationException($"Product {item.ProductId} no longer exists.");

            if (item.Quantity > product.StockQuantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for '{product.Name}'. Requested: {item.Quantity}, available: {product.StockQuantity}.");
            }

            product.StockQuantity -= item.Quantity;

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity
            });
        }

        order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

        db.Orders.Add(order);
        db.CartItems.RemoveRange(cartItems);

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        var itemResponses = order.Items
            .Select(i => new OrderItemResponse(
                i.ProductId,
                i.ProductName,
                i.UnitPrice,
                i.Quantity,
                i.UnitPrice * i.Quantity))
            .ToList();

        return new OrderResponse(
            order.Id,
            order.CustomerId,
            order.CreatedAtUtc,
            itemResponses,
            order.TotalAmount);
    }
}
