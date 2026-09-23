using SimpleCommerce.Application.Abstractions;
using SimpleCommerce.Domain.Entities;
using SimpleCommerce.Domain.Enums;

namespace SimpleCommerce.UnitTests.Application;

// In-memory fakes replacing the Infrastructure repositories, so the
// Application services can be tested without a database.
// Each fake exposes SaveCount so tests can assert when a save happened.

public class FakeClock : IClock
{
    public DateTime UtcNow { get; set; } = new(2026, 9, 23, 12, 0, 0, DateTimeKind.Utc);
}

public class FakeProductRepository : IProductRepository
{
    public List<Product> Products { get; } = new();
    public int SaveCount { get; private set; }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Products.FirstOrDefault(p => p.Id == id));

    public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Products.ToList());

    public Task<List<Product>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default) =>
        Task.FromResult(Products.Where(p => ids.Contains(p.Id)).ToList());

    public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) =>
        Task.FromResult(Products.Any(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase)));

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        Products.Add(product);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }
}

public class FakeVendorRepository : IVendorRepository
{
    public List<Vendor> Vendors { get; } = new();
    public int SaveCount { get; private set; }

    public Task<Vendor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Vendors.FirstOrDefault(v => v.Id == id));

    public Task<List<Vendor>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Vendors.ToList());

    public Task AddAsync(Vendor vendor, CancellationToken cancellationToken = default)
    {
        Vendors.Add(vendor);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }
}

public class FakeOrderRepository : IOrderRepository
{
    public List<Order> Orders { get; } = new();
    public int SaveCount { get; private set; }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Orders.FirstOrDefault(o => o.Id == id));

    public Task<List<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Orders.Where(o => o.CustomerId == customerId).ToList());

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        Orders.Add(order);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }
}

public class FakePaymentRepository : IPaymentRepository
{
    public List<Payment> Payments { get; } = new();
    public int SaveCount { get; private set; }

    public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Payments.FirstOrDefault(p => p.Id == id));

    public Task<bool> HasPendingForOrderAsync(Guid orderId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Payments.Any(p => p.OrderId == orderId && p.Status == PaymentStatus.Pending));

    public Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        Payments.Add(payment);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }
}
