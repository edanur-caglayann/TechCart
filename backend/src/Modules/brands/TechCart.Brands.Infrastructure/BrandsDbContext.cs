using Microsoft.EntityFrameworkCore;
using TechCart.Brands.Domain.Entities;

namespace TechCart.Brands.Infrastructure;

public class BrandsDbContext : DbContext
{
    public BrandsDbContext(DbContextOptions<BrandsDbContext> options) : base(options) { }

    public DbSet<Brand> Brands => Set<Brand>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("brands");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BrandsDbContext).Assembly);
    }
}