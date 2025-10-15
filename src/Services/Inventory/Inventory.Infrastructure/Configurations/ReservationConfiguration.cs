using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Inventory.Infrastructure.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).IsRequired().ValueGeneratedNever();

            builder.Property(r => r.OrderId).IsRequired();

            builder.Property(r => r.StockId).IsRequired();

            builder.Property(r => r.WarehouseId).IsRequired();

            builder.Property(r => r.Sku).IsRequired().HasMaxLength(100);

            builder.Property(r => r.Count).IsRequired();

            builder.Property(r => r.Status).IsRequired().HasConversion<string>();

            builder.Property(r => r.TimeCreate).IsRequired();

            builder.Property(r => r.TimeExpired).IsRequired();

            builder.Property(r => r.TimeCommited);

            builder.Property(r => r.TimeReleased);

            builder.HasOne<Stock>()
                .WithMany()
                .HasForeignKey(r => r.StockId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => r.Sku);
            builder.HasIndex(r => r.OrderId);
            builder.HasIndex(r => r.StockId);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.TimeExpired);

        }
    }
}
