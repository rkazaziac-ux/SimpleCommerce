using SimpleCommerce.Application.Services;
using SimpleCommerce.Domain.Enums;
using SimpleCommerce.UnitTests.Application;
using Xunit;

namespace SimpleCommerce.UnitTests.Application;

public class PaymentServiceTests
{
    private readonly FakePaymentRepository _paymentRepository = new();
    private readonly FakeOrderRepository _orderRepository = new();
    private readonly FakeClock _clock = new();
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _service = new PaymentService(_paymentRepository, _orderRepository, _clock);
    }

    [Fact]
    public async Task CreateForOrderAsync_TakesAmountFromOrderTotal()
    {
        var product = new Product { Code = "P-100", Name = "Laptop", Price = 1000m, StockQuantity = 5 };
        var order = new Order { CustomerId = Guid.NewGuid(), CreatedAtUtc = _clock.UtcNow, Status = OrderStatus.Pending };
        order.Items.Add(new OrderItem { ProductId = product.Id, Product = product, Quantity = 2, UnitPrice = 1000m });
        _orderRepository.Orders.Add(order);

        var result = await _service.CreateForOrderAsync(order.Id);

        Assert.True(result.Succeeded);
        Assert.Equal(2000m, result.Value!.Amount);
        Assert.Equal(PaymentStatus.Pending.ToString(), result.Value.Status);
    }

    [Fact]
    public async Task MarkPaidAsync_SetsPaymentPaidAndOrderPaid()
    {
        var order = new Order { CustomerId = Guid.NewGuid(), CreatedAtUtc = _clock.UtcNow, Status = OrderStatus.Pending };
        _orderRepository.Orders.Add(order);
        var payment = new Payment { OrderId = order.Id, Order = order, Amount = 2000m, Status = PaymentStatus.Pending, CreatedAtUtc = _clock.UtcNow };
        _paymentRepository.Payments.Add(payment);

        var result = await _service.MarkPaidAsync(payment.Id, "TRX-12345");

        Assert.True(result.Succeeded);
        Assert.Equal(PaymentStatus.Paid, payment.Status);
        Assert.Equal("TRX-12345", payment.TransactionRef);
        Assert.Equal(OrderStatus.Paid, order.Status);
    }

    [Fact]
    public async Task MarkPaidAsync_Twice_Fails()
    {
        var order = new Order { CustomerId = Guid.NewGuid(), CreatedAtUtc = _clock.UtcNow, Status = OrderStatus.Pending };
        var payment = new Payment { OrderId = order.Id, Order = order, Amount = 2000m, Status = PaymentStatus.Pending, CreatedAtUtc = _clock.UtcNow };
        _paymentRepository.Payments.Add(payment);

        await _service.MarkPaidAsync(payment.Id, "TRX-1");
        var result = await _service.MarkPaidAsync(payment.Id, "TRX-2");

        Assert.False(result.Succeeded);
        Assert.Equal("TRX-1", payment.TransactionRef); // first payment wins
    }

    [Fact]
    public async Task MarkFailedAsync_KeepsOrderPendingForRetry()
    {
        var order = new Order { CustomerId = Guid.NewGuid(), CreatedAtUtc = _clock.UtcNow, Status = OrderStatus.Pending };
        var payment = new Payment { OrderId = order.Id, Order = order, Amount = 2000m, Status = PaymentStatus.Pending, CreatedAtUtc = _clock.UtcNow };
        _paymentRepository.Payments.Add(payment);

        var result = await _service.MarkFailedAsync(payment.Id);

        Assert.True(result.Succeeded);
        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Equal(OrderStatus.Pending, order.Status); // a new payment can be attempted
    }
}
