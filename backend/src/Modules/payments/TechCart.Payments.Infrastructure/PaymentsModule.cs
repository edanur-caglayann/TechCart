using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCart.Payments.Application.Abstractions;
using TechCart.Payments.Application.InitiateCheckout;
using TechCart.Payments.Application.List;
using TechCart.Payments.Application.ProcessCallback;
using TechCart.Payments.Domain.Repositories;
using TechCart.Payments.Infrastructure.Iyzico;
using TechCart.Payments.Infrastructure.Repositories;

namespace TechCart.Payments.Infrastructure;

public static class PaymentsModule
{
    public static IServiceCollection AddPaymentsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));
        services.Configure<IyzicoOptions>(configuration.GetSection("Iyzico"));
        services.AddScoped<IPaymentGateway, IyzicoCheckoutClient>();        
        services.AddScoped<IPaymentWriteRepository, PaymentWriteRepository>();
        services.AddScoped<InitiateCheckoutHandler>();
        services.AddScoped<ProcessPaymentCallbackHandler>();
        services.AddScoped<IPaymentReadRepository, PaymentReadRepository>();
        services.AddScoped<ListPaymentSummariesHandler>();
        return services;
    }
}