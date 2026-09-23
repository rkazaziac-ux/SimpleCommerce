namespace SimpleCommerce.Application.Services;

public class AuthService : IAuthService
{
    private static readonly string[] AllowedRoles = { "Customer", "Admin" };

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            return Result<AuthResponse>.Fail("A valid email is required.");
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            return Result<AuthResponse>.Fail("Password must be at least 6 characters.");
        if (string.IsNullOrWhiteSpace(request.FullName))
            return Result<AuthResponse>.Fail("Full name is required.");

        var role = ParseRole(request.Role);
        if (role is null)
            return Result<AuthResponse>.Fail("Role must be either 'Customer' or 'Admin'.");

        var email = request.Email.Trim().ToLowerInvariant();
        if (await _userRepository.EmailExistsAsync(email, cancellationToken))
            return Result<AuthResponse>.Fail("A user with this email already exists.");

        var (hash, salt) = _passwordHasher.Hash(request.Password);
        var user = new AppUser
        {
            Email = email,
            FullName = request.FullName.Trim(),
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = role.Value
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Ok(ToResponse(user, _tokenService.CreateToken(user)));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<AuthResponse>.Fail("Email is required.");
        if (string.IsNullOrWhiteSpace(request.Password))
            return Result<AuthResponse>.Fail("Password is required.");

        var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt))
            return Result<AuthResponse>.Fail("Invalid email or password."); // same message for both cases (no user enumeration)

        return Result<AuthResponse>.Ok(ToResponse(user, _tokenService.CreateToken(user)));
    }

    private static UserRole? ParseRole(string? role) =>
        Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsed) && Enum.IsDefined(parsed)
            ? parsed
            : null;

    private static AuthResponse ToResponse(AppUser user, string token) =>
        new(user.Id, user.Email, user.FullName, user.Role.ToString(), token);
}
