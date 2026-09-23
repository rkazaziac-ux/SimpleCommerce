namespace SimpleCommerce.Domain.Entities;

/// <summary>
/// A system user for JWT authentication with a single role (Customer or Admin).
/// Password is stored as PBKDF2 hash + salt — never in plain text.
/// Deliberately a simple custom user instead of full ASP.NET Core Identity,
/// which the spec offers as an either/or choice.
/// </summary>
public class AppUser : BaseEntity
{
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string PasswordSalt { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public UserRole Role { get; set; } = UserRole.Customer;
}
