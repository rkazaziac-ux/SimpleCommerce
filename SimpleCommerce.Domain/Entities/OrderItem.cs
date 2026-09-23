namespace SimpleCommerce.Domain.Entities;

/// <summary>
/// One line of an order: which product, how many units, and the unit price
/// captured at the moment the order was placed.
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public int Quantity { get; set; }

    /// <summary>Product price copied at order time, so later price changes do not affect past orders.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Computed line total = unit price × quantity.</summary>
    public decimal LineTotal => UnitPrice * Quantity;
}
