namespace SimpleCommerce.Domain.Entities.Products.Entitiy;

/// <summary>
/// A sellable product with code, name, price and current stock.
/// </summary>
public class Product : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    /// <summary>Vendors that can supply this product (many-to-many via VendorProduct).</summary>
    public ICollection<VendorProduct> VendorProducts { get; set; } = new List<VendorProduct>();
}
