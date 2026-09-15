using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.Brands.Infrastructure;
using TechCart.Categories.Infrastructure;
using TechCart.ProductImages.Infrastructure;
using TechCart.Products.Infrastructure;
using TechCart.Seeder;
using TechCart.ProductImages.Infrastructure;

if (args.Length == 0)
{
    Console.WriteLine("Kullanım: dotnet run -- <csv-dosya-yolu>");
    return;
}

var csvPath = args[0];

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

// Api'de kullandığımız aynı extension metotları — DbContext kurulumunu
// burada tekrar yazmıyoruz, olduğu gibi yeniden kullanıyoruz.
services.AddCategoriesModule(configuration);
services.AddBrandsModule(configuration);
services.AddProductsModule(configuration);
services.AddProductImagesModule(configuration);
services.AddScoped<ProductSeeder>();


await using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var seeder = scope.ServiceProvider.GetRequiredService<ProductSeeder>();
await seeder.RunAsync(csvPath, CancellationToken.None);