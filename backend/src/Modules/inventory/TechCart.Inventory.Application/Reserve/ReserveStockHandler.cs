using TechCart.Inventory.Application.Dtos.RequestDtos;
using TechCart.Inventory.Domain.Repositories;

namespace TechCart.Inventory.Application.Reserve;

public class ReserveStockHandler(IProductStockWriteRepository productStockWriteRepository)
{
    public Task<bool> Handle(ReserveStockCommand command, CancellationToken ct)
        => productStockWriteRepository.TryReserveAsync(command.ProductId, command.Quantity, ct);
}