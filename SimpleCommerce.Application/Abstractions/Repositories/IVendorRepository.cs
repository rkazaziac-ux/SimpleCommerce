namespace SimpleCommerce.Application.Abstractions.Repositories;

/// <remarks>GetByIdAsync loads the vendor together with its VendorProducts and each product's name.</remarks>
public interface IVendorRepository : IRepository<Vendor>
{
    Task<List<Vendor>> GetAllAsync(CancellationToken cancellationToken = default);
}
