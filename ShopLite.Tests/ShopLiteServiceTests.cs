using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ShopLite.Api.Data;
using ShopLite.Api.DTOs;
using ShopLite.Api.Models;
using ShopLite.Api.Services;

namespace ShopLite.Tests;

public sealed class ShopLiteServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ShopLiteDbContext _db;
    private readonly CartService _cartService;
    private readonly CheckoutService _checkoutService;

    public ShopLiteServiceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ShopLiteDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new ShopLiteDbContext(options);
        _db.Database.EnsureCreated();

        _cartService = new CartService(_db);
        _checkoutService = new CheckoutService(_db);
    }

    [Fact]
    public async Task GetCartAsync_CalculatesTotalCorrectly_ForMultipleItems()
    {
        // Arrange
        var keyboard = new Product { Name = "Keyboard", Price = 100.00m, StockQuantity = 10 };
        var mouse = new Product { Name = "Mouse", Price = 25.50m, StockQuantity = 10 };
        _db.Products.AddRange(keyboard, mouse);
        await _db.SaveChangesAsync();

        await _cartService.AddItemAsync("cust-1", new AddCartItemRequest { ProductId = keyboard.Id, Quantity = 2 }); // 200.00
        await _cartService.AddItemAsync("cust-1", new AddCartItemRequest { ProductId = mouse.Id, Quantity = 3 });    //  76.50

        // Act
        var cart = await _cartService.GetCartAsync("cust-1");

        // Assert
        Assert.Equal(2, cart.Items.Count);
        Assert.Equal(276.50m, cart.Total);
    }

    [Fact]
    public async Task AddItemAsync_ThrowsInvalidOperationException_WhenQuantityExceedsStock()
    {
        // Arrange
        var monitor = new Product { Name = "4K Monitor", Price = 350.00m, StockQuantity = 5 };
        _db.Products.Add(monitor);
        await _db.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cartService.AddItemAsync("cust-1", new AddCartItemRequest { ProductId = monitor.Id, Quantity = 6 }));

        Assert.Contains("Insufficient stock", exception.Message);
    }

    [Fact]
    public async Task CheckoutAsync_ReducesProductStock_AndClearsCustomerCart()
    {
        // Arrange
        var headphones = new Product { Name = "Headphones", Price = 150.00m, StockQuantity = 10 };
        _db.Products.Add(headphones);
        await _db.SaveChangesAsync();

        await _cartService.AddItemAsync("cust-1", new AddCartItemRequest { ProductId = headphones.Id, Quantity = 4 });

        // Act
        var order = await _checkoutService.CheckoutAsync("cust-1");

        // Assert
        Assert.Equal(600.00m, order.TotalAmount);
        Assert.Single(order.Items);

        var updatedProduct = await _db.Products.FindAsync(headphones.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal(6, updatedProduct.StockQuantity);

        var cartAfterCheckout = await _cartService.GetCartAsync("cust-1");
        Assert.Empty(cartAfterCheckout.Items);
        Assert.Equal(0m, cartAfterCheckout.Total);
    }

    [Fact]
    public async Task CheckoutAsync_ThrowsInvalidOperationException_WhenStockBecomesInsufficientBeforeCheckout()
    {
        // Arrange
        var webcam = new Product { Name = "Webcam", Price = 80.00m, StockQuantity = 3 };
        _db.Products.Add(webcam);
        await _db.SaveChangesAsync();

        await _cartService.AddItemAsync("cust-1", new AddCartItemRequest { ProductId = webcam.Id, Quantity = 3 });

        // Simulate another customer buying 2 units before cust-1 checks out
        webcam.StockQuantity = 1;
        await _db.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _checkoutService.CheckoutAsync("cust-1"));

        Assert.Contains("Insufficient stock", exception.Message);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
