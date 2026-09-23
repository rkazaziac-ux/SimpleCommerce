using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SimpleCommerce.Infrastructure.Persistence;

/// <summary>
/// Design-time factory so `dotnet ef migrations` can create AppDbContext without
/// running the API host (the DbContext lives in this class library and the API's
/// Program.cs registers it at runtime via AddInfrastructure).
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=epdsql;Database=SimpleCommerceDb;User Id=sa;Password=***REPLACE***;TrustServerCertificate=True;MultipleActiveResultSets=true");

        return new AppDbContext(optionsBuilder.Options);
    }
}
