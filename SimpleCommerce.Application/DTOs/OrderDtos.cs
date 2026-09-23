namespace SimpleCommerce.Application.DTOs;

// ---------- Order ----------

public record OrderItemDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal);

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    string Status,
    DateTime CreatedAtUtc,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items);

public record CreateOrderItemRequest(Guid ProductId, int Quantity);

public class CreateOrderRequest
{
    public CreateOrderRequest(List<CreateOrderItemRequest> items)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
    }

    public List<CreateOrderItemRequest> Items { get; }
}

// ---------- Payment (simulated) ----------

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    string Status,
    string? TransactionRef,
    DateTime CreatedAtUtc,
    DateTime? PaidAtUtc);
