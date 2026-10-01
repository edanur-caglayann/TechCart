using Microsoft.EntityFrameworkCore;
using TechCart.Inventory.Domain.Entities;

namespace TechCart.Inventory.Infrastructure;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<ProductStock> ProductStocks => Set<ProductStock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("inventory");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
    }
}