using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TechCart.CartItems.Infrastructure;

public static class CartItemsModule
{
    public static IServiceCollection AddCartItemsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CartItemsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        return services;
    }
}