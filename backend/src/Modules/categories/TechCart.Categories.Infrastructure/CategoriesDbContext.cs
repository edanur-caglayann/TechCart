using Microsoft.EntityFrameworkCore;
using TechCart.Categories.Domain.Entities;

namespace TechCart.Categories.Infrastructure;

public class CategoriesDbContext : DbContext
{
    public CategoriesDbContext(DbContextOptions<CategoriesDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("categories");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CategoriesDbContext).Assembly);
    }
}