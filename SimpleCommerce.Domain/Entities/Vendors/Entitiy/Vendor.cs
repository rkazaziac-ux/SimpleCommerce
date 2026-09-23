namespace SimpleCommerce.Domain.Entities.Vendors.Entitiy;

/// <summary>
/// A vendor that supplies products, with contact information.
/// </summary>
public class Vendor : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }

    /// <summary>Products this vendor supplies (many-to-many via VendorProduct).</summary>
    public ICollection<VendorProduct> VendorProducts { get; set; } = new List<VendorProduct>();
}
