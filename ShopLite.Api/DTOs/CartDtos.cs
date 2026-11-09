using System.ComponentModel.DataAnnotations;

namespace ShopLite.Api.DTOs;

public record AddCartItemRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ProductId { get; init; }

    [Required]
    [Range(1, 1000)]
    public int Quantity { get; init; }
}

public record CartItemResponse(
    int ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal
);

public record CartResponse(
    string CustomerId,
    IReadOnlyList<CartItemResponse> Items,
    decimal Total
);
