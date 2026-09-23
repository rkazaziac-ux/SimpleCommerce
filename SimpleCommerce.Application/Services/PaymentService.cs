namespace SimpleCommerce.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IClock _clock;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IClock clock)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _clock = clock;
    }

    public async Task<Result<PaymentDto>> CreateForOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
            return Result<PaymentDto>.Fail("Order was not found.");
        if (order.Status != OrderStatus.Pending)
            return Result<PaymentDto>.Fail("Only a pending order can be paid.");
        if (await _paymentRepository.HasPendingForOrderAsync(orderId, cancellationToken))
            return Result<PaymentDto>.Fail("A pending payment already exists for this order.");

        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.Items.Sum(i => i.LineTotal),
            Status = PaymentStatus.Pending,
            CreatedAtUtc = _clock.UtcNow
        };

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return Result<PaymentDto>.Ok(ToDto(payment));
    }

    public async Task<Result<PaymentDto>> MarkPaidAsync(Guid paymentId, string transactionRef, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(transactionRef))
            return Result<PaymentDto>.Fail("Transaction reference is required.");

        var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment is null)
            return Result<PaymentDto>.Fail("Payment was not found.");
        if (payment.Status != PaymentStatus.Pending)
            return Result<PaymentDto>.Fail("Only a pending payment can be marked as paid.");

        payment.Status = PaymentStatus.Paid;
        payment.TransactionRef = transactionRef.Trim();
        payment.PaidAtUtc = _clock.UtcNow;

        if (payment.Order is not null)
            payment.Order.Status = OrderStatus.Paid;

        await _paymentRepository.SaveChangesAsync(cancellationToken);
        return Result<PaymentDto>.Ok(ToDto(payment));
    }

    public async Task<Result<PaymentDto>> MarkFailedAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment is null)
            return Result<PaymentDto>.Fail("Payment was not found.");
        if (payment.Status != PaymentStatus.Pending)
            return Result<PaymentDto>.Fail("Only a pending payment can be marked as failed.");

        payment.Status = PaymentStatus.Failed;
        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return Result<PaymentDto>.Ok(ToDto(payment));
    }

    private static PaymentDto ToDto(Payment payment) => new(
        payment.Id,
        payment.OrderId,
        payment.Amount,
        payment.Status.ToString(),
        payment.TransactionRef,
        payment.CreatedAtUtc,
        payment.PaidAtUtc);
}
