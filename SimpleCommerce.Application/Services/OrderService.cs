
namespace SimpleCommerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IClock _clock;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IClock clock)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _clock = clock;
    }

    public async Task<Result<OrderDto>> CreateAsync(Guid customerId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (customerId == Guid.Empty)
            return Result<OrderDto>.Fail("Customer is required.");
        if (request.Items is null || request.Items.Count == 0)
            return Result<OrderDto>.Fail("Order must contain at least one item.");
        if (request.Items.Any(i => i.ProductId == Guid.Empty))
            return Result<OrderDto>.Fail("Each order item needs a product.");
        if (request.Items.Select(i => i.ProductId).Distinct().Count() != request.Items.Count)
            return Result<OrderDto>.Fail("Duplicate products in one order are not allowed. Combine their quantities instead.");

        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        var order = new Order
        {
            CustomerId = customerId,
            CreatedAtUtc = _clock.UtcNow,
            Status = OrderStatus.Pending
        };

        foreach (var item in request.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product is null)
                return Result<OrderDto>.Fail($"Product '{item.ProductId}' was not found.");
            if (item.Quantity <= 0)
                return Result<OrderDto>.Fail($"Quantity for product '{product.Name}' must be greater than zero.");
            if (product.StockQuantity < item.Quantity)
                return Result<OrderDto>.Fail(
                    $"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, requested: {item.Quantity}.");

            product.StockQuantity -= item.Quantity; // deduct at order time
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price // price snapshot at order time
            });
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken); // RowVersion makes concurrent deductions safe

        return Result<OrderDto>.Ok(ToDto(order));
    }

    public async Task<Result<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        return order is null
            ? Result<OrderDto>.Fail("Order was not found.")
            : Result<OrderDto>.Ok(ToDto(order));
    }

    public async Task<List<OrderDto>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByCustomerAsync(customerId, cancellationToken);
        return orders.Select(ToDto).ToList();
    }

    public async Task<Result<OrderDto>> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
            return Result<OrderDto>.Fail("Order was not found.");
        if (order.Status != OrderStatus.Pending)
            return Result<OrderDto>.Fail("Only a pending order can be cancelled.");

        foreach (var item in order.Items)
        {
            if (item.Product is not null)
                item.Product.StockQuantity += item.Quantity; // restock
        }

        order.Status = OrderStatus.Cancelled;
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return Result<OrderDto>.Ok(ToDto(order));
    }

    private static OrderDto ToDto(Order order) => new(
        order.Id,
        order.CustomerId,
        order.Status.ToString(),
        order.CreatedAtUtc,
        order.Items.Sum(i => i.LineTotal),
        order.Items
            .Select(i => new OrderItemDto(i.ProductId, i.Product?.Name ?? string.Empty, i.Quantity, i.UnitPrice, i.LineTotal))
            .ToList());
}
