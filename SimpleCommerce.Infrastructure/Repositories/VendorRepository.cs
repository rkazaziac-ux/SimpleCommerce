using Microsoft.EntityFrameworkCore;
using SimpleCommerce.Application.Abstractions.Repositories;
using SimpleCommerce.Domain.Entities;
using SimpleCommerce.Infrastructure.Persistence;

namespace SimpleCommerce.Infrastructure.Repositories;

public class VendorRepository : IVendorRepository
{
    private readonly AppDbContext _context;

    public VendorRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>Loads the vendor with its VendorProducts and each product's name.</summary>
    public Task<Vendor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Vendors
            .Include(v => v.VendorProducts)
                .ThenInclude(vp => vp.Product)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public Task<List<Vendor>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _context.Vendors
            .AsNoTracking()
            .Include(v => v.VendorProducts)
                .ThenInclude(vp => vp.Product)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Vendor vendor, CancellationToken cancellationToken = default)
    {
        await _context.Vendors.AddAsync(vendor, cancellationToken);
    }

    /// <summary>
    /// Inserts the supply link with an explicit Added state. Adding it through the
    /// vendor's navigation made EF emit UPDATE for the new child row (the graph
    /// tracker treated it as an existing entity), which failed with
    /// DbUpdateConcurrencyException on a row that did not exist yet.
    /// </summary>
    public async Task AddProductLinkAsync(VendorProduct link, CancellationToken cancellationToken = default)
    {
        await _context.VendorProducts.AddAsync(link, cancellationToken);
        _context.Entry(link).State = EntityState.Added;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
