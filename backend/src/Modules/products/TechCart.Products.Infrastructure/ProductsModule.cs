using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.Products.Application.Abstractions;
using TechCart.Products.Application.Detail;
using TechCart.Products.Application.Facets;
using TechCart.Products.Application.List;
using TechCart.Products.Application.Suggestions;
using TechCart.Products.Application.Summaries;
using TechCart.Products.Domain.Repositories;
using TechCart.Products.Infrastructure.Repositories;

namespace TechCart.Products.Infrastructure;

public static class ProductsModule
{
    public static IServiceCollection AddProductsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProductsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
        services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
        services.AddScoped<IProductReadRepository, ProductReadRepository>(); 

        services.AddScoped<ListProductsHandler>();          
        services.AddScoped<GetProductFacetsHandler>();       
        services.AddScoped<GetProductSuggestionsHandler>();  
        services.AddScoped<GetProductDetailHandler>();     
        services.AddScoped<GetProductSummariesHandler>();
        return services;
    }
}