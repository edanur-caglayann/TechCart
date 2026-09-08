using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.Infrastructure.Persistence;

namespace TechCart.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTechCartInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5433;Database=techcart;Username=techcart;Password=techcart";

        services.AddDbContext<TechCartDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsAssembly(typeof(TechCartDbContext).Assembly.FullName)));

        return services;
    }
}
