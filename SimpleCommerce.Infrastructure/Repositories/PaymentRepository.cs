using Microsoft.EntityFrameworkCore;
using SimpleCommerce.Application.Abstractions.Repositories;
using SimpleCommerce.Domain.Entities;
using SimpleCommerce.Domain.Enums;
using SimpleCommerce.Infrastructure.Persistence;

namespace SimpleCommerce.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>Loads the payment with its order so MarkPaid can also flip the order status.</summary>
    public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> HasPendingForOrderAsync(Guid orderId, CancellationToken cancellationToken = default) =>
        _context.Payments.AnyAsync(p => p.OrderId == orderId && p.Status == PaymentStatus.Pending, cancellationToken);

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
