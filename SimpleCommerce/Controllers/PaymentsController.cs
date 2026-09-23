namespace SimpleCommerce.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController(IPaymentService paymentService) : ApiControllerBase
{
    /// <summary>Registers a simulated payment for a pending order (owner or Admin).</summary>
    [HttpPost("order/{orderId:guid}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateForOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await paymentService.CreateForOrderAsync(orderId, cancellationToken);
        return result.Succeeded ? Ok(result.Value)
            : result.Error!.Contains("was not found") ? NotFound(new { error = result.Error })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>Simulates a successful gateway callback: payment + order become Paid (Admin only).</summary>
    [HttpPost("{id:guid}/mark-paid")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkPaid(Guid id, [FromBody] MarkPaidRequest request, CancellationToken cancellationToken)
    {
        var result = await paymentService.MarkPaidAsync(id, request.TransactionRef, cancellationToken);
        return result.Succeeded ? Ok(result.Value)
            : result.Error!.Contains("was not found") ? NotFound(new { error = result.Error })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>Simulates a failed gateway attempt; the order stays pending for a retry (Admin only).</summary>
    [HttpPost("{id:guid}/mark-failed")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkFailed(Guid id, CancellationToken cancellationToken)
    {
        var result = await paymentService.MarkFailedAsync(id, cancellationToken);
        return result.Succeeded ? Ok(result.Value)
            : result.Error!.Contains("was not found") ? NotFound(new { error = result.Error })
            : BadRequest(new { error = result.Error });
    }
}

public record MarkPaidRequest(string TransactionRef);
