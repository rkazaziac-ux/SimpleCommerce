
namespace SimpleCommerce.Domain.Entities.Orders.Repository;

/// <remarks>GetByIdAsync loads the order together with its items and each item's product.</remarks>
public interface IOrderRepository : IRepository<Order>
{
    Task<List<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
}
