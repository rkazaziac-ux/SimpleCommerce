namespace SimpleCommerce.Domain.Entities.Payments.Repository;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<bool> HasPendingForOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
}
