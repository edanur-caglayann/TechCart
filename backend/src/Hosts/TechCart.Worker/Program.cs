using Microsoft.Extensions.DependencyInjection;
using TechCart.Inventory.Infrastructure;
using TechCart.OrderItems.Infrastructure;
using TechCart.Orders.Infrastructure;
using TechCart.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddOrdersModule(builder.Configuration);
builder.Services.AddOrderItemsModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);
builder.Services.AddHostedService<ExpiredOrdersBackgroundService>();

builder.ConfigureContainer(new DefaultServiceProviderFactory(
    new ServiceProviderOptions
    {
        ValidateOnBuild = false,
        ValidateScopes = false
    }));

var host = builder.Build();
host.Run();