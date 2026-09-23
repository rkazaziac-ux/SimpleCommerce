using Microsoft.EntityFrameworkCore;
using SimpleCommerce.Application.Abstractions.Repositories;
using SimpleCommerce.Domain.Entities;
using SimpleCommerce.Infrastructure.Persistence;

namespace SimpleCommerce.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Products.AsNoTracking().ToListAsync(cancellationToken);

    public Task<List<Product>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default) =>
        _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);

    public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) =>
        _context.Products.AnyAsync(p => p.Code == code, cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
