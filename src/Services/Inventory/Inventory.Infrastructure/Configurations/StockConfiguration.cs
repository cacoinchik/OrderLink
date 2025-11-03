using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Inventory.Infrastructure.Configurations
{
    public class StockConfiguration : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder.ToTable("Stocks");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).IsRequired().ValueGeneratedNever();

            builder.Property(s => s.WarehouseId).IsRequired();

            builder.Property(s => s.Sku).IsRequired().HasMaxLength(100);

            builder.Property(s => s.AvailableQuantity).IsRequired();

            builder.Property(s => s.ReservedQuantity).IsRequired();

            builder.Property(s => s.TotalQuantity).IsRequired();

            builder.Property(s => s.TimeUpdate).IsRequired();

            builder.Property(s => s.RowVersion).IsRowVersion().IsConcurrencyToken();

            builder.HasOne<Warehouse>()
                .WithMany()
                .HasForeignKey(s => s.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.Sku);
            builder.HasIndex(s => s.WarehouseId);

            builder.HasIndex(s => new { s.WarehouseId, s.Sku }).IsUnique();
        }
    }
}
