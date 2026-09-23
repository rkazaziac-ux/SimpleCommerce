namespace SimpleCommerce.Application.Abstractions.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<bool> HasPendingForOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
}
