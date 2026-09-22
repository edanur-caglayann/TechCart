using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.Inventory.Application.Abstractions;
using TechCart.Inventory.Application.Confirm;
using TechCart.Inventory.Application.Release;
using TechCart.Inventory.Application.Reserve;
using TechCart.Inventory.Domain.Repositories;
using TechCart.Inventory.Infrastructure.Repositories;

namespace TechCart.Inventory.Infrastructure;

public static class InventoryModule
{
    public static IServiceCollection AddInventoryModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped<IProductStockWriteRepository, ProductStockWriteRepository>();
        services.AddScoped<IProductStockReadRepository, ProductStockReadRepository>();
        services.AddScoped<ReserveStockHandler>();
        services.AddScoped<ConfirmReservationHandler>();
        services.AddScoped<ReleaseReservationHandler>();

        return services;
    }
}