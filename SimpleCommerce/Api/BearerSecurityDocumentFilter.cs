using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SimpleCommerce.Api;

/// <summary>
/// Injects the global Bearer security requirement directly into the OpenAPI document.
/// Used instead of AddSecurityRequirement because with Microsoft.OpenApi v2 the
/// lambda overload serializes to an empty "security": [ { } ] and Swagger UI then
/// sends no Authorization header even when the user is marked authorized.
/// </summary>
public class BearerSecurityDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        document.Security ??= new List<OpenApiSecurityRequirement>();

        document.Security.Add(new OpenApiSecurityRequirement
        {
            // The hostDocument overload makes the reference serialize to a proper $ref
            // (without it, Microsoft.OpenApi v2 writes an empty object).
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });
    }
}
