using Microsoft.EntityFrameworkCore;
using TechCart.CartItems.Domain.Entities;

namespace TechCart.CartItems.Infrastructure;

public class CartItemsDbContext : DbContext
{
    public CartItemsDbContext(DbContextOptions<CartItemsDbContext> options) : base(options) { }

    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cart_items");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartItemsDbContext).Assembly);
    }
}