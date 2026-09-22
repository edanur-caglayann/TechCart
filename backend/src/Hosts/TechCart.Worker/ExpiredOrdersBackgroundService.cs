using TechCart.Orders.Application.CancelExpiredOrders;

namespace TechCart.Worker;

public class ExpiredOrdersBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<ExpiredOrdersBackgroundService> logger)
    : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<CancelExpiredOrdersHandler>();
                var cancelledCount = await handler.Handle(stoppingToken);

                if (cancelledCount > 0)
                    logger.LogInformation("{Count} süresi dolmuş sipariş iptal edildi.", cancelledCount);
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}