namespace SimpleCommerce.Application.Abstractions;

/// <summary>Abstraction over the system time so services stay unit-testable.</summary>
public interface IClock
{
    DateTime UtcNow { get; }
}

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
