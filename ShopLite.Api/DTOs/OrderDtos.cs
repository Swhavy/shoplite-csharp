namespace ShopLite.Api.DTOs;

public record OrderItemResponse(
    int ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal
);

public record OrderResponse(
    int OrderId,
    string CustomerId,
    DateTime CreatedAtUtc,
    IReadOnlyList<OrderItemResponse> Items,
    decimal TotalAmount
);
