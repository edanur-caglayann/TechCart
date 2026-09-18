namespace TechCart.Addresses.Application.Dtos.RequestDtos;

public record UpdateAddressCommand(Guid AddressId, Guid UserId, string Title, string FullName,
    string Phone, string City, string District, string Neighborhood, string AddressLine, string PostalCode);
