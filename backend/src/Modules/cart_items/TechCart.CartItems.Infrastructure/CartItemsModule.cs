using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.CartItems.Application.Abstractions;
using TechCart.CartItems.Application.Add;
using TechCart.CartItems.Application.Clear;
using TechCart.CartItems.Application.Get;
using TechCart.CartItems.Application.Merge;
using TechCart.CartItems.Application.Remove;
using TechCart.CartItems.Application.Update;
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
        services.AddScoped<GetCartHandler>();
        services.AddScoped<AddCartItemHandler>();
        services.AddScoped<UpdateCartItemQuantityHandler>();
        services.AddScoped<RemoveCartItemHandler>();
        services.AddScoped<ClearCartHandler>();
        services.AddScoped<MergeCartHandler>();
        return services;
    }
}