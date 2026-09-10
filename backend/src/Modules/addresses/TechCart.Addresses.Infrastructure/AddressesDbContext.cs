using Microsoft.EntityFrameworkCore;
using TechCart.Addresses.Domain.Entities;

namespace TechCart.Addresses.Infrastructure;

// Adres modulunun EF Core ile db'ye erismesini saglar
public class AddressesDbContext : DbContext
{
    public AddressesDbContext(DbContextOptions<AddressesDbContext> options) : base(options) { }

    public DbSet<Address> Addresses => Set<Address>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("addresses");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AddressesDbContext).Assembly);
    }
}