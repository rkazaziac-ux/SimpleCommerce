namespace SimpleCommerce.Domain.Entities.Vendors.Repository;

/// <remarks>GetByIdAsync loads the vendor together with its VendorProducts and each product's name.</remarks>
public interface IVendorRepository : IRepository<Vendor>
{
    Task<List<Vendor>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Inserts a new vendor-product supply link as a NEW row (state = Added explicitly).</summary>
    Task AddProductLinkAsync(VendorProduct link, CancellationToken cancellationToken = default);
}
