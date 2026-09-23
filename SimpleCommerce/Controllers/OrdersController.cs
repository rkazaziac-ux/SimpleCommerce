namespace SimpleCommerce.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(IOrderService orderService) : ApiControllerBase
{
    /// <summary>Places an order for the logged-in user. Stock is deducted at order time.</summary>
    [HttpPost]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var customerId = GetUserId();
        if (customerId == Guid.Empty)
            return Unauthorized();

        var result = await orderService.CreateAsync(customerId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>Gets one order (its owner, or an Admin).</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await orderService.GetByIdAsync(id, cancellationToken);
        if (!result.Succeeded)
            return NotFound(new { error = result.Error });

        if (result.Value!.CustomerId != GetUserId() && !User.IsInRole("Admin"))
            return Forbid(); // customers can only see their own orders

        return Ok(result.Value);
    }

    /// <summary>Lists the logged-in customer's own orders.</summary>
    [HttpGet("my")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderDto>>> MyOrders(CancellationToken cancellationToken) =>
        Ok(await orderService.GetByCustomerAsync(GetUserId(), cancellationToken));

    /// <summary>Cancels a pending order and restocks every item (owner or Admin).</summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await orderService.GetByIdAsync(id, cancellationToken);
        if (!result.Succeeded)
            return NotFound(new { error = result.Error });

        if (result.Value!.CustomerId != GetUserId() && !User.IsInRole("Admin"))
            return Forbid();

        var cancelResult = await orderService.CancelAsync(id, cancellationToken);
        return cancelResult.Succeeded ? Ok(cancelResult.Value) : BadRequest(new { error = cancelResult.Error });
    }
}
