using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.CartItems.Application.Abstractions;
using TechCart.CartItems.Domain.Repositories;
using TechCart.CartItems.Infrastructure.Repositories;

namespace TechCart.CartItems.Infrastructure;

public static class CartItemsModule
{
    public static IServiceCollection AddCartItemsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CartItemsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped<ICartItemWriteRepository, CartItemWriteRepository>();
        services.AddScoped<ICartItemReadRepository, CartItemReadRepository>();

        return services;
    }
}