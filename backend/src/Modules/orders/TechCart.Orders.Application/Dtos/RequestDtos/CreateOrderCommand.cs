namespace TechCart.Orders.Application.Dtos.RequestDtos;

public record CreateOrderCommand(Guid UserId, Guid AddressId);