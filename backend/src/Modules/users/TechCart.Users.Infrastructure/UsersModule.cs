using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Login;
using TechCart.Users.Application.Profile;
using TechCart.Users.Application.Register;
using TechCart.Users.Domain.Repositories;
using TechCart.Users.Infrastructure.Repositories;
using TechCart.Users.Infrastructure.Security;

namespace TechCart.Users.Infrastructure;

// Users modülünün tüm servislerini tek yerden DI container'a kaydeden extension method.
public static class UsersModule
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));
        
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services.AddScoped<IUserWriteRepository, UserWriteRepository>();
        services.AddScoped<IUserReadRepository, UserReadRepository>(); 
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>(); 
        services.AddScoped<LoginHandler>(); 
        services.AddScoped<GetMyProfileHandler>();   
        services.AddScoped<UpdateProfileHandler>(); 
        services.AddScoped<ChangePasswordHandler>();
        return services;
    }
}