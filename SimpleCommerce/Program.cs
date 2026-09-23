using SimpleCommerce.Api;
using SimpleCommerce.Application;
using SimpleCommerce.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// One entry point per layer - Program.cs only composes them.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

// Create the database (if missing) and seed the default admin account.
await app.Services.SeedAdminAsync(app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
