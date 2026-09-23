namespace SimpleCommerce.Domain.Enums;

/// <summary>
/// The two roles required by the spec:
/// Admin manages products/vendors, Customer places orders.
/// </summary>
public enum UserRole
{
    Customer = 1,
    Admin = 2
}
