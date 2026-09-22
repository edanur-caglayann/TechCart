using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.OrderItems.Application.Create;
using TechCart.OrderItems.Application.List;
using TechCart.OrderItems.Domain.Repositories;
using TechCart.OrderItems.Infrastructure.Repositories;

namespace TechCart.OrderItems.Infrastructure;

public static class OrderItemsModule
{
    public static IServiceCollection AddOrderItemsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderItemsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));
        
        services.AddScoped<IOrderItemWriteRepository, OrderItemWriteRepository>();
        services.AddScoped<CreateOrderItemsHandler>();
        services.AddScoped<IOrderItemReadRepository, OrderItemReadRepository>();
        services.AddScoped<ListOrderItemsHandler>();
        return services;
    }
}