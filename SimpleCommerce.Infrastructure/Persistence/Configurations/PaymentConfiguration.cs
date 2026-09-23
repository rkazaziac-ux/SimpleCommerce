namespace SimpleCommerce.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
               .HasPrecision(18, 2);

        builder.Property(p => p.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(p => p.TransactionRef)
               .HasMaxLength(100);

        builder.Property(p => p.CreatedAtUtc)
               .IsRequired();

        builder.HasOne(p => p.Order)
               .WithMany()
               .HasForeignKey(p => p.OrderId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.OrderId);
    }
}
