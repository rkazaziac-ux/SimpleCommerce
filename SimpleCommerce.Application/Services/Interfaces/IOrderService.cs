namespace SimpleCommerce.Application.Services.Interfaces;

public interface IOrderService
{
    /// <summary>
    /// Places an order: validates stock, deducts it at order time, snapshots unit prices
    /// and persists everything in one save. RowVersion on Product guards against
    /// concurrent orders making the stock negative.
    /// </summary>
    Task<Result<OrderDto>> CreateAsync(Guid customerId, CreateOrderRequest request, CancellationToken cancellationToken = default);

    Task<Result<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<OrderDto>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>Cancels a pending order and returns every item's quantity to stock.</summary>
    Task<Result<OrderDto>> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
