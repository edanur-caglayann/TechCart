using TechCart.OrderItems.Application.Dtos.RequestDtos;
using TechCart.OrderItems.Contracts.ResponseDtos;
using TechCart.OrderItems.Domain.Repositories;

namespace TechCart.OrderItems.Application.List;

public class ListOrderItemsHandler
{
    private readonly IOrderItemReadRepository _orderItemReadRepository;

    public ListOrderItemsHandler(IOrderItemReadRepository orderItemReadRepository)
        => _orderItemReadRepository = orderItemReadRepository;

    public Task<List<OrderItemDto>> Handle(ListOrderItemsQuery query, CancellationToken ct)
        => _orderItemReadRepository.GetByOrderIdAsync(query.OrderId, ct);
}