namespace SimpleCommerce.Application.Abstractions.Repositories;


/// <summary>
/// Generic *contract* (an interface, not a base class) for the three operations every
/// </summary>
public interface IRepository<TEntity> : ISaveChanges where TEntity : BaseEntity
{
    /// <summary>Loads one aggregate by id (Infrastructure decides which navigations to include).</summary>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
}
