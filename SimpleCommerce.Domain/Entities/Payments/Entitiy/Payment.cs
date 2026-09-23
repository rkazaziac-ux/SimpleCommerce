namespace SimpleCommerce.Domain.Entities.Payments.Entitiy;

/// <summary>
/// Simulated payment record for an order. No real payment gateway is connected.
/// </summary>
public class Payment : AuditableBaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionRef { get; set; }

    public DateTime? PaidAtUtc { get; set; }
}
