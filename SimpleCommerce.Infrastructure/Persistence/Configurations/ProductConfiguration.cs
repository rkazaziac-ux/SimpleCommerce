using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleCommerce.Domain.Entities;

namespace SimpleCommerce.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", t =>
            t.HasCheckConstraint("CK_Products_Stock_NonNegative", "[StockQuantity] >= 0"));

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(p => p.Code)
               .IsUnique(); // product code must be unique

        builder.Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(p => p.Price)
               .HasPrecision(18, 2);

        // Concurrency
        builder.Property(p => p.StockQuantity)
               .IsConcurrencyToken();
    }
}
