using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.Brands.Application.Abstractions;
using TechCart.Brands.Application.List;
using TechCart.Brands.Domain.Repositories;
using TechCart.Brands.Infrastructure.Repositories;

namespace TechCart.Brands.Infrastructure;

public static class BrandsModule
{
    public static IServiceCollection AddBrandsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BrandsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped<IBrandWriteRepository, BrandWriteRepository>();
        services.AddScoped<IBrandReadRepository, BrandReadRepository>();

        services.AddScoped<ListBrandsHandler>();

        return services;
    }
}