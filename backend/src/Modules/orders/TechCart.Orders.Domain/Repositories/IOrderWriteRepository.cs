using TechCart.Orders.Domain.Entities;

namespace TechCart.Orders.Domain.Repositories;

public interface IOrderWriteRepository
{
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
    Task<bool> TryMarkAsPaidAsync(Guid orderId, CancellationToken ct);
    // Worker'ın ihtiyacı: belirtilen zamandan once oluşturulmuş, hâlâ
    // AwaitingPayment durumundaki tüm siparişleri  getirir.
    Task<List<Order>> GetExpiredAwaitingPaymentOrdersAsync(DateTime cutoffTime, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    
}