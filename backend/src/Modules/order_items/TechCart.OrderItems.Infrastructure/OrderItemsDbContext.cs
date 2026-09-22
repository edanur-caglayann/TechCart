using Microsoft.EntityFrameworkCore;
using TechCart.OrderItems.Domain.Entities;

namespace TechCart.OrderItems.Infrastructure;

public class OrderItemsDbContext : DbContext
{
    public OrderItemsDbContext(DbContextOptions<OrderItemsDbContext> options) : base(options) { }

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("order_items");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderItemsDbContext).Assembly);
    }
}