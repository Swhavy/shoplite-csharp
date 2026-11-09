using Microsoft.EntityFrameworkCore;
using ShopLite.Api.Models;

namespace ShopLite.Api.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ShopLiteDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.Products.AnyAsync())
        {
            return;
        }

        Product[] sampleProducts =
        [
            new() { Name = "Mechanical Keyboard", Price = 129.99m, StockQuantity = 15 },
            new() { Name = "Wireless Gaming Mouse", Price = 59.50m, StockQuantity = 25 },
            new() { Name = "27-inch 4K Monitor", Price = 349.00m, StockQuantity = 8 },
            new() { Name = "USB-C Docking Station", Price = 89.99m, StockQuantity = 20 },
            new() { Name = "Noise-Cancelling Headphones", Price = 199.95m, StockQuantity = 12 }
        ];

        await db.Products.AddRangeAsync(sampleProducts);
        await db.SaveChangesAsync();
    }
}
