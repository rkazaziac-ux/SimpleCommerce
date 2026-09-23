namespace SimpleCommerce.Domain.Common.Abstractions;

/// <summary>
/// Base class for all domain entities.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

}
