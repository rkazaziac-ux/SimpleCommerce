namespace SimpleCommerce.Application.Abstractions;

/// <summary>Hashes passwords (implemented with PBKDF2 in Infrastructure).</summary>
public interface IPasswordHasher
{
    (string Hash, string Salt) Hash(string password);
    bool Verify(string password, string hash, string salt);
}

/// <summary>Creates signed JWT access tokens for authenticated users.</summary>
public interface ITokenService
{
    string CreateToken(AppUser user);
}
