using TechCart.OrderItems.Domain.Entities;
using TechCart.OrderItems.Domain.Repositories;

namespace TechCart.OrderItems.Infrastructure.Repositories;

public class OrderItemWriteRepository(OrderItemsDbContext dbContext) : IOrderItemWriteRepository
{
    public async Task AddRangeAsync(IEnumerable<OrderItem> orderItems, CancellationToken ct)
        => await dbContext.OrderItems.AddRangeAsync(orderItems, ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => dbContext.SaveChangesAsync(ct);
}