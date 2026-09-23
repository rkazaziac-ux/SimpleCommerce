namespace SimpleCommerce.Domain.Entities.Vendors.Entitiy;

/// <summary>
/// Join entity for the many-to-many Vendor ↔ Product relationship.
/// SupplyPrice lets the same product be supplied by different vendors at different prices.
/// </summary>
public class VendorProduct : BaseEntity
{
    public Guid VendorId { get; set; }
    public Vendor Vendor { get; set; } = default!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    /// <summary>The price at which this vendor supplies the product.</summary>
    public decimal SupplyPrice { get; set; }
}
