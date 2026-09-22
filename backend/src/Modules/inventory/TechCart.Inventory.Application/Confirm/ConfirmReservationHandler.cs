using TechCart.Inventory.Application.Dtos.RequestDtos;
using TechCart.Inventory.Domain.Repositories;

namespace TechCart.Inventory.Application.Confirm;

public class ConfirmReservationHandler(IProductStockWriteRepository productStockWriteRepository)
{
    public Task<int> Handle(ConfirmReservationCommand command, CancellationToken ct)
        => productStockWriteRepository.ConfirmReservationAsync(command.ProductId, command.Quantity, ct);
     }