using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace SimpleCommerce.Api;

/// <summary>
/// Single entry point for everything presentation-level: controllers, JWT authentication
/// and Swagger. Program.cs stays minimal, mirroring the other layers:
///
///   builder.Services.AddApplication();
///   builder.Services.AddInfrastructure(builder.Configuration);
///   builder.Services.AddApi(builder.Configuration);
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Controllers + DbUpdateConcurrencyException -> 409 Conflict filter
        // (two simultaneous orders deducting the same stock).
        services.AddControllers(options => options.Filters.Add<ConcurrencyExceptionFilter>());

        services.AddOpenApi();

        // JWT Bearer authentication; the role claim (Customer/Admin) drives [Authorize(Roles = ...)].
        var jwt = configuration.GetSection("Jwt");
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwt["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
                    ValidateLifetime = true
                };
            });

        services.AddAuthorization();

        // Swagger with a JWT Authorize button so every endpoint can be tested by hand.
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpleCommerce API", Version = "v1" });
            // ApiKey-in-header format is used deliberately: with Swashbuckle 10 +
            // Microsoft.OpenApi v2, SecuritySchemeType.Http sets the "Authorized" state
            // but does not actually attach the header to requests. ApiKey works reliably.
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste the JWT from /api/auth/login **including** the 'Bearer ' prefix."
            });
            // The requirement is injected by BearerSecurityDocumentFilter - see its remarks
            // for why AddSecurityRequirement is not used here.
            options.DocumentFilter<BearerSecurityDocumentFilter>();
        });

        return services;
    }
}
