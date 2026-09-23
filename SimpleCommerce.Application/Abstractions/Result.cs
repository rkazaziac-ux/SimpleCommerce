namespace SimpleCommerce.Application.Abstractions;

/// <summary>
/// Lightweight Result pattern: business-rule failures (e.g. insufficient stock) are returned
/// as a failed result instead of thrown exceptions, so the API layer can map them to
/// proper HTTP responses easily.
/// </summary>
public record Result<T>(bool Succeeded, T? Value, string? Error)
{
    public static Result<T> Ok(T value) => new(true, value, null);

    public static Result<T> Fail(string error) => new(false, default, error);
}
