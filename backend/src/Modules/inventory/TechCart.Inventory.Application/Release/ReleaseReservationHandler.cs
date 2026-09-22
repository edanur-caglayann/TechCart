using TechCart.Inventory.Application.Dtos.RequestDtos;
using TechCart.Inventory.Domain.Repositories;

namespace TechCart.Inventory.Application.Release;

public class ReleaseReservationHandler(IProductStockWriteRepository productStockWriteRepository)
{
    public Task Handle(ReleaseReservationCommand command, CancellationToken ct)
        => productStockWriteRepository.ReleaseReservationAsync(command.ProductId, command.Quantity, ct);
}