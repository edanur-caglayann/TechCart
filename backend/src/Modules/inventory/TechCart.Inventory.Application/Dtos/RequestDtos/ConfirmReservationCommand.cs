namespace TechCart.Inventory.Application.Dtos.RequestDtos;

public record ConfirmReservationCommand(Guid ProductId, int Quantity);