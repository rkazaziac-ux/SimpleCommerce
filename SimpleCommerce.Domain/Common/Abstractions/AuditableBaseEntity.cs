namespace SimpleCommerce.Domain.Common.Abstractions;

/// <summary>
/// Base class for entities that carry a creation timestamp (e.g. Order, Payment).
/// </summary>
public abstract class AuditableBaseEntity : BaseEntity
{
    public DateTime CreatedAtUtc { get; set; }
}
