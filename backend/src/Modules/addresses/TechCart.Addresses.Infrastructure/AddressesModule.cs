using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.Addresses.Application.Abstractions;
using TechCart.Addresses.Application.Create;
using TechCart.Addresses.Application.Delete;
using TechCart.Addresses.Application.List;
using TechCart.Addresses.Application.SetDefault;
using TechCart.Addresses.Application.Update;
using TechCart.Addresses.Domain.Repositories;
using TechCart.Addresses.Infrastructure.Repositories;

namespace TechCart.Addresses.Infrastructure;

public static class AddressesModule
{
    public static IServiceCollection AddAddressesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AddressesDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped<IAddressWriteRepository, AddressWriteRepository>();
        services.AddScoped<IAddressReadRepository, AddressReadRepository>();

        services.AddScoped<ListMyAddressesHandler>();
        services.AddScoped<CreateAddressHandler>();
        services.AddScoped<UpdateAddressHandler>();
        services.AddScoped<DeleteAddressHandler>();
        services.AddScoped<SetDefaultAddressHandler>();

        return services;
    }
}