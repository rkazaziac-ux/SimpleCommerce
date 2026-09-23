using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleCommerce.Domain.Entities;

namespace SimpleCommerce.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerId)
               .IsRequired();

        builder.Property(o => o.Status)
               .HasConversion<string>() // enum as string for readability
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(o => o.CreatedAtUtc)
               .IsRequired();

        // Customer is an AppUser (Users table). The Domain only knows a Guid (CustomerId);
        // the referential integrity is wired here, without coupling the Domain to the user concept.
        builder.HasOne<AppUser>()
               .WithMany()
               .HasForeignKey(o => o.CustomerId)
               .OnDelete(DeleteBehavior.Restrict); // cannot delete a user who has orders

        builder.HasMany(o => o.Items)
               .WithOne(i => i.Order)
               .HasForeignKey(i => i.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
