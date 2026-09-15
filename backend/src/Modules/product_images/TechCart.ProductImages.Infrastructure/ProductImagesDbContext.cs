using Microsoft.EntityFrameworkCore;
using TechCart.ProductImages.Domain.Entities;

namespace TechCart.ProductImages.Infrastructure;

public class ProductImagesDbContext : DbContext
{
    public ProductImagesDbContext(DbContextOptions<ProductImagesDbContext> options) : base(options) { }

    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("product_images");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductImagesDbContext).Assembly);
    }
}