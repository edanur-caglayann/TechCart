namespace TechCart.Addresses.Application.Dtos.ResponseDtos;

public record AddressDto(
    Guid Id, string Title, string FullName, string Phone,
    string City, string District, string Neighborhood, string AddressLine, string PostalCode, bool IsDefault);
