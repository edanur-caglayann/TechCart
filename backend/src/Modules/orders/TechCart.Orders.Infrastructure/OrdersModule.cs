using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.Orders.Application.CancelExpiredOrders;
using TechCart.Orders.Application.CreateOrder;
using TechCart.Orders.Application.ListMyOrders;
using TechCart.Orders.Domain.Repositories;
using TechCart.Orders.Infrastructure.Repositories;

namespace TechCart.Orders.Infrastructure;

public static class OrdersModule
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped<IOrderWriteRepository, OrderWriteRepository>();
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<CancelExpiredOrdersHandler>();
        services.AddScoped<ListMyOrdersHandler>();
        return services;
    }
}