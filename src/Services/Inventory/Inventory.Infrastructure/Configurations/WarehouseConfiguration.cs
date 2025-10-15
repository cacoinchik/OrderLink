using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Configurations
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id).IsRequired().ValueGeneratedNever();

            builder.Property(w => w.Name).IsRequired().HasMaxLength(200);

            builder.Property(w => w.Region).IsRequired().HasMaxLength(100);

            builder.Property(w => w.City).IsRequired().HasMaxLength(100);

            builder.Property(w => w.Address).IsRequired().HasMaxLength(250);

            builder.Property(w => w.IsActive).IsRequired();

            builder.Property(w => w.TimeCreate).IsRequired();

            builder.HasIndex(w => w.Name);


        }
    }
}
