namespace SimpleCommerce.Application.Services.Interfaces;

public interface IPaymentService
{
    /// <summary>Registers a simulated payment for a pending order (no real gateway).</summary>
    Task<Result<PaymentDto>> CreateForOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>Simulates the gateway callback: marks the payment paid and the order paid.</summary>
    Task<Result<PaymentDto>> MarkPaidAsync(Guid paymentId, string transactionRef, CancellationToken cancellationToken = default);

    /// <summary>Simulates a failed gateway attempt; the order stays pending and can be retried.</summary>
    Task<Result<PaymentDto>> MarkFailedAsync(Guid paymentId, CancellationToken cancellationToken = default);
}
