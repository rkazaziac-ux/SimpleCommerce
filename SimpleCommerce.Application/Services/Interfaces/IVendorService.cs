namespace SimpleCommerce.Application.Services.Interfaces;

public interface IVendorService
{
    Task<Result<VendorDto>> CreateAsync(CreateVendorRequest request, CancellationToken cancellationToken = default);
    Task<Result<VendorDto>> UpdateContactAsync(Guid id, UpdateVendorContactRequest request, CancellationToken cancellationToken = default);
    /// <summary>Links an existing product to a vendor with a vendor-specific supply price (many-to-many).</summary>
    Task<Result<VendorDto>> AddProductAsync(Guid vendorId, AddVendorProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<VendorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<VendorDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
