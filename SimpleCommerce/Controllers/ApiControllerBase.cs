using System.Security.Claims;

namespace SimpleCommerce.Controllers;

/// <summary>
/// Shared base for all API controllers — common helpers like reading the
/// current user's id from the JWT claims.
/// </summary>
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Reads the logged-in user's id from the JWT (NameIdentifier claim).</summary>
    protected Guid GetUserId() =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
}
