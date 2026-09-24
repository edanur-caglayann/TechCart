using Microsoft.EntityFrameworkCore;
using TechCart.Orders.Domain.Entities;
using TechCart.Orders.Domain.Enums;
using TechCart.Orders.Domain.Repositories;

namespace TechCart.Orders.Infrastructure.Repositories;

public class OrderWriteRepository(OrdersDbContext dbContext) : IOrderWriteRepository
{
    public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct)
        => dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct);

    public async Task AddAsync(Order order, CancellationToken ct)
        => await dbContext.Orders.AddAsync(order, ct);

    // siparis hala odeme bekliyorsa Paid'e cevir
    public async Task<bool> TryMarkAsPaidAsync(Guid orderId, CancellationToken ct)
    {
        var affectedRows = await dbContext.Orders
            .Where(o => o.Id == orderId && o.Status == OrderStatus.AwaitingPayment)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(o => o.Status, OrderStatus.Paid)
                .SetProperty(o => o.UpdatedAt, DateTime.UtcNow), ct);

        return affectedRows > 0;
    }
    
    public Task<List<Order>> GetExpiredAwaitingPaymentOrdersAsync(DateTime cutoffTime, CancellationToken ct)
        => dbContext.Orders
            .Where(o => o.Status == OrderStatus.AwaitingPayment && o.CreatedAt < cutoffTime)
            .ToListAsync(ct);
    
    public Task<List<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => dbContext.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    
    public Task SaveChangesAsync(CancellationToken ct)
        => dbContext.SaveChangesAsync(ct);
}