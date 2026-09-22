namespace TechCart.Addresses.Application.Dtos.RequestDtos;

public record SetDefaultAddressCommand(Guid AddressId, Guid UserId);
