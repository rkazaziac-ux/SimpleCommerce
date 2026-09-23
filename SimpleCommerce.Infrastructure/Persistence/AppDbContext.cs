using Microsoft.EntityFrameworkCore;
using SimpleCommerce.Domain.Entities;

namespace SimpleCommerce.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext. All entity mapping lives in Persistence/Configurations/*
/// (one IEntityTypeConfiguration per entity) and is applied automatically.
/// Every repository in a request shares this single context, so one SaveChanges
/// call commits everything atomically.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<VendorProduct> VendorProducts => Set<VendorProduct>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
