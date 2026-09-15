using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.Brands.Infrastructure;
using TechCart.Categories.Infrastructure;
using TechCart.Inventory.Infrastructure;
using TechCart.ProductImages.Infrastructure;
using TechCart.Products.Infrastructure;
using TechCart.Seeder;

if (args.Length == 0)
{
    Console.WriteLine("Kullanım:");
    Console.WriteLine("  Ürün içe aktarma: dotnet run -- <csv-dosya-yolu>");
    Console.WriteLine("  Renk geri doldurma: dotnet run -- backfill-colors");
    Console.WriteLine("  Stok kopyalama: dotnet run -- backfill-stock"); // YENİ
    return;
}

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

services.AddCategoriesModule(configuration);
services.AddBrandsModule(configuration);
services.AddProductsModule(configuration);
services.AddProductImagesModule(configuration);
services.AddInventoryModule(configuration);
services.AddScoped<ProductSeeder>();
services.AddScoped<ColorBackfiller>();
services.AddScoped<ProductStockReplicator>(); 

await using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

if (args[0] == "backfill-colors")
{
    var colorBackfiller = scope.ServiceProvider.GetRequiredService<ColorBackfiller>();
    await colorBackfiller.RunAsync(CancellationToken.None);
}
else if (args[0] == "backfill-stock") 
{
    var stockReplicator = scope.ServiceProvider.GetRequiredService<ProductStockReplicator>();
    await stockReplicator.RunAsync(CancellationToken.None);
}
else
{
    var seeder = scope.ServiceProvider.GetRequiredService<ProductSeeder>();
    await seeder.RunAsync(args[0], CancellationToken.None);
}