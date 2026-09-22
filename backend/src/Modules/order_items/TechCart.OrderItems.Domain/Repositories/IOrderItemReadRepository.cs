using TechCart.OrderItems.Contracts.ResponseDtos;

namespace TechCart.OrderItems.Domain.Repositories;

public interface IOrderItemReadRepository
{
    Task<List<OrderItemDto>> GetByOrderIdAsync(Guid orderId, CancellationToken ct);
}