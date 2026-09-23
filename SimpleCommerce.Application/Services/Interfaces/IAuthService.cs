namespace SimpleCommerce.Application.Services.Interfaces;

public interface IAuthService
{
    /// <summary>Registers a new user with the Customer or Admin role.</summary>
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>Validates credentials and returns a signed JWT.</summary>
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
