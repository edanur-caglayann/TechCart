using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.ProductImages.Application.Abstractions;
using TechCart.ProductImages.Domain.Repositories;
using TechCart.ProductImages.Infrastructure.Repositories;

namespace TechCart.ProductImages.Infrastructure;

public static class ProductImagesModule
{
    public static IServiceCollection AddProductImagesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProductImagesDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped<IProductImageWriteRepository, ProductImageWriteRepository>();
        services.AddScoped<IProductImageWriteRepository, ProductImageWriteRepository>();
        services.AddScoped<IProductImageReadRepository, ProductImageReadRepository>(); 
        return services;
    }
}