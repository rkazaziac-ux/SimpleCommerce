namespace SimpleCommerce.Application.Abstractions.Repositories;

/// <summary>
/// Shared capability contract for persisting tracked changes — NOT a base repository.
/// </summary>
public interface ISaveChanges
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
