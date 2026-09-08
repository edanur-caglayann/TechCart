using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TechCart.Infrastructure.Persistence;

public sealed class TechCartDbContextFactory : IDesignTimeDbContextFactory<TechCartDbContext>
{
    public TechCartDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("TECHCART_CONNECTION_STRING")
            ?? "Host=localhost;Port=5433;Database=techcart;Username=techcart;Password=techcart";

        var optionsBuilder = new DbContextOptionsBuilder<TechCartDbContext>();
        optionsBuilder.UseNpgsql(
            connectionString,
            options => options.MigrationsAssembly(typeof(TechCartDbContext).Assembly.FullName));

        return new TechCartDbContext(optionsBuilder.Options);
    }
}
