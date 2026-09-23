using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleCommerce.Application.Abstractions.Repositories;
using SimpleCommerce.Infrastructure.Persistence;
using SimpleCommerce.Infrastructure.Repositories;

namespace SimpleCommerce.Infrastructure;

/// <summary>
/// Single entry point for registering every Infrastructure service with the
/// ASP.NET Core IoC container. In Program.cs only this one call is referenced:
///
///   builder.Services.AddInfrastructure(builder.Configuration);
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.EnableRetryOnFailure(3); // transient fault handling
                    sql.CommandTimeout(30);
                }));

        // One shared scoped DbContext behind every repository -> one atomic SaveChanges.
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }
}
