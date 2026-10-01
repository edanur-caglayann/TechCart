using Microsoft.EntityFrameworkCore;
using TechCart.Users.Domain.Entities;

namespace TechCart.Users.Infrastructure;

// Users modülüne özel DbContext. Diğer modüllerin DbContext'leriyle (Orders, Payments vb.)
// KARIŞMIYOR — her modül kendi DbContext'ini, kendi tablolarını yönetiyor (modül izolasyonu).
public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("users");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
    }
}
