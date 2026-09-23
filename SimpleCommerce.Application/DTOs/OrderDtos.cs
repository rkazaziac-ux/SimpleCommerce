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

public record CreateOrderRequest(IReadOnlyList<CreateOrderItemRequest> Items);

// ---------- Payment (simulated) ----------

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    string Status,
    string? TransactionRef,
    DateTime CreatedAtUtc,
    DateTime? PaidAtUtc);
