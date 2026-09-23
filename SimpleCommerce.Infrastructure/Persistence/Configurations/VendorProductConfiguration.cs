namespace SimpleCommerce.Infrastructure.Persistence.Configurations;

public class VendorProductConfiguration : IEntityTypeConfiguration<VendorProduct>
{
    public void Configure(EntityTypeBuilder<VendorProduct> builder)
    {
        builder.ToTable("VendorProducts");

        builder.HasKey(vp => vp.Id);

        builder.Property(vp => vp.SupplyPrice)
               .HasPrecision(18, 2);

        builder.HasOne(vp => vp.Vendor)
               .WithMany(v => v.VendorProducts)
               .HasForeignKey(vp => vp.VendorId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vp => vp.Product)
               .WithMany(p => p.VendorProducts)
               .HasForeignKey(vp => vp.ProductId)
               .OnDelete(DeleteBehavior.Restrict); // a supplied product cannot just be deleted

        // A vendor supplies each product at most once.
        builder.HasIndex(vp => new { vp.VendorId, vp.ProductId })
               .IsUnique();
    }
}
