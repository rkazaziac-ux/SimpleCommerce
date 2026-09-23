namespace SimpleCommerce.Domain.Common.Abstractions;

/// <summary>
/// Shared capability contract for persisting tracked changes — NOT a base repository.
/// </summary>
public interface ISaveChanges
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
