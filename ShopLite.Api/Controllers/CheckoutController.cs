using Microsoft.AspNetCore.Mvc;
using ShopLite.Api.DTOs;
using ShopLite.Api.Services;

namespace ShopLite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController(ICheckoutService checkoutService) : ControllerBase
{
    [HttpPost("{customerId}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderResponse>> Checkout(string customerId, CancellationToken ct)
    {
        try
        {
            var order = await checkoutService.CheckoutAsync(customerId, ct);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Checkout failed",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}
