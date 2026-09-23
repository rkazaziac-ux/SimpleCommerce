using SimpleCommerce.Application.Services;
using SimpleCommerce.Domain.Enums;
using SimpleCommerce.UnitTests.Application;
using Xunit;

namespace SimpleCommerce.UnitTests.Application;

public class OrderServiceTests
{
    private readonly FakeProductRepository _productRepository = new();
    private readonly FakeOrderRepository _orderRepository = new();
    private readonly FakeClock _clock = new();
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _service = new OrderService(_orderRepository, _productRepository, _clock);
    }

    [Fact]
    public async Task CreateAsync_DeductsStockAndCalculatesTotal()
    {
        var laptop = new Product { Code = "P-100", Name = "Laptop", Price = 1000m, StockQuantity = 10 };
        var mouse = new Product { Code = "P-200", Name = "Mouse", Price = 250m, StockQuantity = 20 };
        _productRepository.Products.Add(laptop);
        _productRepository.Products.Add(mouse);

        var request = new CreateOrderRequest(
        [
            new CreateOrderItemRequest(laptop.Id, 2), // 2 × 1000 = 2000
            new CreateOrderItemRequest(mouse.Id, 3)   // 3 ×  250 =  750
        ]);

        var result = await _service.CreateAsync(Guid.NewGuid(), request);

        Assert.True(result.Succeeded);
        Assert.Equal(2750m, result.Value!.TotalAmount);            // total calculation
        Assert.Equal(8, laptop.StockQuantity);                     // stock deducted
        Assert.Equal(17, mouse.StockQuantity);                     // stock deducted
        Assert.Equal(1000m, result.Value.Items[0].UnitPrice);      // price snapshot
        Assert.Equal(OrderStatus.Pending.ToString(), result.Value.Status);
        Assert.Equal(1, _orderRepository.SaveCount);
    }

    [Fact]
    public async Task CreateAsync_WithInsufficientStock_FailsAndKeepsStock()
    {
        var laptop = new Product { Code = "P-100", Name = "Laptop", Price = 1000m, StockQuantity = 10 };
        _productRepository.Products.Add(laptop);

        var result = await _service.CreateAsync(Guid.NewGuid(),
            new CreateOrderRequest([new CreateOrderItemRequest(laptop.Id, 11)]));

        Assert.False(result.Succeeded);
        Assert.Contains("Insufficient stock", result.Error);
        Assert.Equal(10, laptop.StockQuantity); // stock unchanged, never negative
        Assert.Equal(0, _orderRepository.SaveCount);
    }

    [Fact]
    public async Task CreateAsync_WithUnknownProduct_Fails()
    {
        var result = await _service.CreateAsync(Guid.NewGuid(),
            new CreateOrderRequest([new CreateOrderItemRequest(Guid.NewGuid(), 1)]));

        Assert.False(result.Succeeded);
        Assert.Contains("was not found", result.Error);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateProducts_Fails()
    {
        var laptop = new Product { Code = "P-100", Name = "Laptop", Price = 1000m, StockQuantity = 10 };
        _productRepository.Products.Add(laptop);

        var result = await _service.CreateAsync(Guid.NewGuid(), new CreateOrderRequest(
        [
            new CreateOrderItemRequest(laptop.Id, 1),
            new CreateOrderItemRequest(laptop.Id, 2)
        ]));

        Assert.False(result.Succeeded);
        Assert.Contains("Duplicate products", result.Error);
    }

    [Fact]
    public async Task CancelAsync_RestocksAllItemsAndCancels()
    {
        var laptop = new Product { Code = "P-100", Name = "Laptop", Price = 1000m, StockQuantity = 10 };
        var mouse = new Product { Code = "P-200", Name = "Mouse", Price = 250m, StockQuantity = 20 };
        _productRepository.Products.Add(laptop);
        _productRepository.Products.Add(mouse);

        var order = new Order { CustomerId = Guid.NewGuid(), CreatedAtUtc = _clock.UtcNow, Status = OrderStatus.Pending };
        order.Items.Add(new OrderItem { ProductId = laptop.Id, Product = laptop, Quantity = 2, UnitPrice = laptop.Price });
        order.Items.Add(new OrderItem { ProductId = mouse.Id, Product = mouse, Quantity = 3, UnitPrice = mouse.Price });
        _orderRepository.Orders.Add(order);
        laptop.StockQuantity -= 2; // as deducted when ordered
        mouse.StockQuantity -= 3;

        var result = await _service.CancelAsync(order.Id);

        Assert.True(result.Succeeded);
        Assert.Equal(OrderStatus.Cancelled.ToString(), result.Value!.Status);
        Assert.Equal(10, laptop.StockQuantity); // restocked
        Assert.Equal(20, mouse.StockQuantity);  // restocked
    }

    [Fact]
    public async Task CancelAsync_WhenOrderIsNotPending_Fails()
    {
        var order = new Order { CustomerId = Guid.NewGuid(), CreatedAtUtc = _clock.UtcNow, Status = OrderStatus.Paid };
        _orderRepository.Orders.Add(order);

        var result = await _service.CancelAsync(order.Id);

        Assert.False(result.Succeeded);
        Assert.Equal(OrderStatus.Paid, order.Status);
    }
}
