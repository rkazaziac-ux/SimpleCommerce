namespace SimpleCommerce.Application.Services;

public class VendorService : IVendorService
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IProductRepository _productRepository;

    public VendorService(IVendorRepository vendorRepository, IProductRepository productRepository)
    {
        _vendorRepository = vendorRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<VendorDto>> CreateAsync(CreateVendorRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<VendorDto>.Fail("Vendor name is required.");
        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<VendorDto>.Fail("Vendor email is required.");

        var vendor = new Vendor
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber?.Trim()
        };

        await _vendorRepository.AddAsync(vendor, cancellationToken);
        await _vendorRepository.SaveChangesAsync(cancellationToken);

        return Result<VendorDto>.Ok(ToDto(vendor));
    }

    public async Task<Result<VendorDto>> UpdateContactAsync(Guid id, UpdateVendorContactRequest request, CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            return Result<VendorDto>.Fail("Vendor was not found.");
        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<VendorDto>.Fail("Vendor email is required.");

        vendor.Email = request.Email.Trim();
        vendor.PhoneNumber = request.PhoneNumber?.Trim();
        await _vendorRepository.SaveChangesAsync(cancellationToken);

        return Result<VendorDto>.Ok(ToDto(vendor));
    }

    public async Task<Result<VendorDto>> AddProductAsync(Guid vendorId, AddVendorProductRequest request, CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorRepository.GetByIdAsync(vendorId, cancellationToken);
        if (vendor is null)
            return Result<VendorDto>.Fail("Vendor was not found.");

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result<VendorDto>.Fail("Product was not found.");
        if (request.SupplyPrice < 0)
            return Result<VendorDto>.Fail("Supply price cannot be negative.");

        if (vendor.VendorProducts.Any(vp => vp.ProductId == product.Id))
            return Result<VendorDto>.Fail($"Vendor '{vendor.Name}' already supplies product '{product.Name}'.");

        // Add the link through the repository (explicit Added state) so EF inserts it
        // instead of walking the vendor graph and issuing an UPDATE for the new row.
        await _vendorRepository.AddProductLinkAsync(new VendorProduct
        {
            VendorId = vendor.Id,
            ProductId = product.Id,
            SupplyPrice = request.SupplyPrice
        }, cancellationToken);

        await _vendorRepository.SaveChangesAsync(cancellationToken);
        return Result<VendorDto>.Ok(ToDto(vendor));
    }

    public async Task<Result<VendorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id, cancellationToken);
        return vendor is null
            ? Result<VendorDto>.Fail("Vendor was not found.")
            : Result<VendorDto>.Ok(ToDto(vendor));
    }

    public async Task<List<VendorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var vendors = await _vendorRepository.GetAllAsync(cancellationToken);
        return vendors.Select(ToDto).ToList();
    }

    private static VendorDto ToDto(Vendor vendor) => new(
        vendor.Id,
        vendor.Name,
        vendor.Email,
        vendor.PhoneNumber,
        vendor.VendorProducts
            .Select(vp => new VendorProductDto(vp.ProductId, vp.Product?.Name ?? string.Empty, vp.SupplyPrice))
            .ToList());
}
