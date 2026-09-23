namespace SimpleCommerce.Application.Abstractions.Repositories;

/// <summary>
/// Follows the same IRepository&lt;AppUser&gt; contract as the other aggregates.
/// Only the email-specific operations are declared here.
/// </summary>
public interface IUserRepository : IRepository<AppUser>
{
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
