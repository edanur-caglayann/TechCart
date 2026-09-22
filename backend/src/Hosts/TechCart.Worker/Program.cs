using TechCart.Inventory.Infrastructure;
using TechCart.OrderItems.Infrastructure;
using TechCart.Orders.Infrastructure;
using TechCart.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddOrdersModule(builder.Configuration);
builder.Services.AddOrderItemsModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);
builder.Services.AddHostedService<ExpiredOrdersBackgroundService>();
var host = builder.Build();
host.Run();
