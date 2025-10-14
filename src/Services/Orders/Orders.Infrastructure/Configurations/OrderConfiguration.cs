using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id).IsRequired().ValueGeneratedNever();

            builder.Property(o => o.CustomerId).IsRequired().HasMaxLength(100);

            builder.Property(o => o.Status).IsRequired().HasConversion<string>();

            builder.Property(o => o.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(o => o.Currency).IsRequired().HasMaxLength(3);

            builder.Property(o => o.TimeCreate).IsRequired();

            builder.Property(o => o.TimeUpdate);

            builder.Property(o => o.ShippingCountry).IsRequired().HasMaxLength(100);

            builder.Property(o => o.ShippingPostalCode).IsRequired().HasMaxLength(20);

            builder.Property(o => o.ShippingCity).IsRequired().HasMaxLength(100);

            builder.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(250);

            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(id => id.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.CustomerId);
            builder.HasIndex(o => o.Status);
        }
    }
}
