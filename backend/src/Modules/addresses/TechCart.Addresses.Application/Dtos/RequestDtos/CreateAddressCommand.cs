namespace TechCart.Addresses.Application.Dtos.RequestDtos;

public record CreateAddressCommand(Guid UserId, string Title, string FullName, string Phone,
    string City, string District, string Neighborhood, string AddressLine, string PostalCode);
