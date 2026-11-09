namespace ShopLite.Api.Models;

public class CartItem
{
    public int Id { get; set; }
    public required string CustomerId { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
}
