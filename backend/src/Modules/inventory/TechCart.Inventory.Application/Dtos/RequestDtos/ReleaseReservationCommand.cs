namespace TechCart.Inventory.Application.Dtos.RequestDtos;

public record ReleaseReservationCommand(Guid ProductId, int Quantity);