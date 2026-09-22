using TechCart.OrderItems.Domain.Entities;

namespace TechCart.OrderItems.Domain.Repositories;

public interface IOrderItemWriteRepository
{
    // AddRangeAsync —  bir sipariş genelde birden fazla satırla
    // (sepetteki her ürün için bir tane) aynı anda oluşuyor,
    // tek tek eklemek yerine toplu ekliyoruz.
    Task AddRangeAsync(IEnumerable<OrderItem> orderItems, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}