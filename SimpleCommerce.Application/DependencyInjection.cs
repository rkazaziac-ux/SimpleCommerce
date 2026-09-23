using Microsoft.Extensions.DependencyInjection;

namespace SimpleCommerce.Application;

/// <summary>
/// Single entry point for registering every Application-layer service with the
/// ASP.NET Core IoC container. In Program.cs only this one call is referenced:
///
///   builder.Services.AddApplication();
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IVendorService, VendorService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
