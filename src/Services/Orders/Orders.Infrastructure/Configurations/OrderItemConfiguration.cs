using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Id).IsRequired().ValueGeneratedNever();

            builder.Property(oi => oi.OrderId).IsRequired();

            builder.Property(oi => oi.Sku).IsRequired().HasMaxLength(100);

            builder.Property(oi => oi.Price).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.Currency).IsRequired().HasMaxLength(3);

            builder.HasIndex(oi => oi.Sku);

            builder.HasIndex(oi => oi.OrderId);
        }
    }
}
