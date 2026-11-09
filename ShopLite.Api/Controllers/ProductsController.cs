using Microsoft.AspNetCore.Mvc;
using ShopLite.Api.DTOs;
using ShopLite.Api.Services;

namespace ShopLite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(
        [FromQuery] string? search,
        CancellationToken ct)
    {
        var products = await productService.GetAllAsync(search, ct);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetById(int id, CancellationToken ct)
    {
        var product = await productService.GetByIdAsync(id, ct);
        if (product is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Product not found",
                Detail = $"Product with ID {id} does not exist.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken ct)
    {
        var created = await productService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
