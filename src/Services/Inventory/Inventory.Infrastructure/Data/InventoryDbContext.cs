using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {

        }

        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<Reservation> Reservations => Set<Reservation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
        }
    }
}
