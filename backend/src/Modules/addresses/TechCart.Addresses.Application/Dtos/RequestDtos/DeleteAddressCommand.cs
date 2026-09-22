namespace TechCart.Addresses.Application.Dtos.RequestDtos;

public record DeleteAddressCommand(Guid AddressId, Guid UserId);
