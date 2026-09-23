using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleCommerce.Application.Abstractions;
using SimpleCommerce.Application.Abstractions.Repositories;
using SimpleCommerce.Domain.Entities;
using SimpleCommerce.Domain.Enums;
using SimpleCommerce.Infrastructure.Persistence;
using SimpleCommerce.Infrastructure.Repositories;
using SimpleCommerce.Infrastructure.Security;

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

        // Repositories (one shared scoped DbContext behind all of them -> atomic SaveChanges).
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Security.
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }

    /// <summary>
    /// Optional (default: on): creates the database if missing and seeds one admin
    /// account (admin@simplecommerce.local / Admin@12345) so the API is testable out
    /// of the box. Inactive via configuration "SeedAdmin": false.
    /// </summary>
    public static async Task SeedAdminAsync(this IServiceProvider serviceProvider, IConfiguration configuration)
    {
        // Default is on; explicitly set "SeedAdmin": false to disable.
        if (bool.TryParse(configuration["SeedAdmin"], out var seed) && !seed)
            return;

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.EnsureCreatedAsync();

        if (!await context.Users.AnyAsync())
        {
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var (hash, salt) = hasher.Hash("Admin@12345");

            context.Users.Add(new AppUser
            {
                Email = "admin@simplecommerce.local",
                FullName = "System Admin",
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = UserRole.Admin
            });
            await context.SaveChangesAsync();
        }
    }
}
