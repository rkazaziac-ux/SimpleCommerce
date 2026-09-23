namespace SimpleCommerce.Domain.Entities;

/// <summary>
/// A customer order consisting of one or more order items.
/// </summary>
public class Order : AuditableBaseEntity
{
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>Order lines (one per product).</summary>
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
