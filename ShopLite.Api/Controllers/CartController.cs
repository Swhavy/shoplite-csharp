using Microsoft.AspNetCore.Mvc;
using ShopLite.Api.DTOs;
using ShopLite.Api.Services;

namespace ShopLite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController(ICartService cartService) : ControllerBase
{
    [HttpGet("{customerId}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartResponse>> GetCart(string customerId, CancellationToken ct)
    {
        var cart = await cartService.GetCartAsync(customerId, ct);
        return Ok(cart);
    }

    [HttpPost("{customerId}/items")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartResponse>> AddItem(
        string customerId,
        [FromBody] AddCartItemRequest request,
        CancellationToken ct)
    {
        try
        {
            var cart = await cartService.AddItemAsync(customerId, request, ct);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Product not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Cannot add item to cart",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}
