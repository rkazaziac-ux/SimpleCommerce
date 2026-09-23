namespace SimpleCommerce.Application.DTOs;

// ---------- Product ----------

public record ProductDto(Guid Id, string Code, string Name, decimal Price, int StockQuantity);

public record CreateProductRequest(string Code, string Name, decimal Price, int StockQuantity);

public record UpdateProductRequest(string Name, decimal Price);

// ---------- Vendor ----------

public record VendorDto(Guid Id, string Name, string Email, string? PhoneNumber, IReadOnlyList<VendorProductDto> Products);

public record VendorProductDto(Guid ProductId, string ProductName, decimal SupplyPrice);

public record CreateVendorRequest(string Name, string Email, string? PhoneNumber);

public record UpdateVendorContactRequest(string Email, string? PhoneNumber);

/// <summary>Lets a vendor supply an existing product at a vendor-specific price (many-to-many).</summary>
public record AddVendorProductRequest(Guid ProductId, decimal SupplyPrice);
