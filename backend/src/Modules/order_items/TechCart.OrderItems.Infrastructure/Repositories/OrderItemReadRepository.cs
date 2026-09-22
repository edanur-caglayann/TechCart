using Microsoft.EntityFrameworkCore;
using TechCart.OrderItems.Contracts.ResponseDtos;
using TechCart.OrderItems.Domain.Repositories;

namespace TechCart.OrderItems.Infrastructure.Repositories;

public class OrderItemReadRepository : IOrderItemReadRepository
{
    private readonly OrderItemsDbContext _dbContext;

    public OrderItemReadRepository(OrderItemsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<OrderItemDto>> GetByOrderIdAsync(Guid orderId, CancellationToken ct)
        => _dbContext.OrderItems.AsNoTracking()
            .Where(i => i.OrderId == orderId)
            .Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.ProductModel,
                i.Quantity, i.UnitPrice, i.VatRate, i.VatAmount))
            .ToListAsync(ct);
}