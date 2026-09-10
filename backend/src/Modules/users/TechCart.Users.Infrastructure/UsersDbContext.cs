using Microsoft.EntityFrameworkCore;
using TechCart.Users.Domain.Entities;

namespace TechCart.Users.Infrastructure;

// Users modülüne özel DbContext. Diğer modüllerin DbContext'leriyle (Orders, Payments vb.)
// KARIŞMIYOR — her modül kendi DbContext'ini, kendi tablolarını yönetiyor (modül izolasyonu).
public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tüm Users tablolarını ayrı bir Postgres şemasında tut ("users" şeması).
        // Aynı fiziksel veritabanını paylaşsak da, tablo mülkiyeti şema seviyesinde net kalıyor.
        modelBuilder.HasDefaultSchema("users");

        // Bu assembly içindeki tüm IEntityTypeConfiguration<T> sınıflarını (UserConfiguration gibi)
        // otomatik bulup uygular — her yeni entity eklediğinde burayı elle güncellemene gerek yok.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
    }
}