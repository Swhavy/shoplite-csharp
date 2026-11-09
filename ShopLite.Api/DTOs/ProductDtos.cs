using System.ComponentModel.DataAnnotations;

namespace ShopLite.Api.DTOs;

public record ProductResponse(
    int Id,
    string Name,
    decimal Price,
    int StockQuantity
);

public record CreateProductRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999.99")]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; init; }
}
