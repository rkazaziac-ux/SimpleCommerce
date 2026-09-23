using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleCommerce.Domain.Entities;

namespace SimpleCommerce.Infrastructure.Persistence.Configurations;

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.ToTable("Vendors");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(v => v.Email)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(v => v.PhoneNumber)
               .HasMaxLength(50);
    }
}
