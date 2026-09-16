using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.Categories.Application.Abstractions;
using TechCart.Categories.Application.List;
using TechCart.Categories.Domain.Repositories;
using TechCart.Categories.Infrastructure.Repositories;

namespace TechCart.Categories.Infrastructure;

public static class CategoriesModule
{
    public static IServiceCollection AddCategoriesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CategoriesDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped<ICategoryWriteRepository, CategoryWriteRepository>();
        services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();

        services.AddScoped<ListCategoriesHandler>();

        return services;
    }
}