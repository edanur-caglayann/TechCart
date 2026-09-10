namespace TechCart.Addresses.Application.Abstractions;

public record AddressDto(
    Guid Id, string Title, string FullName, string Phone,
    string City, string District,string Neighborhood, string AddressLine, string PostalCode, bool IsDefault);

public interface IAddressReadRepository
{
    // verilen userId degerine ait tum adresleri AddressDto listesi olarak getirir
    Task<List<AddressDto>> GetMyAddressesAsync(Guid userId, CancellationToken ct);
}