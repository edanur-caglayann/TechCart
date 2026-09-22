using TechCart.Addresses.Application.Dtos.ResponseDtos;

namespace TechCart.Addresses.Application.Abstractions;

public interface IAddressReadRepository
{
    // verilen userId degerine ait tum adresleri AddressDto listesi olarak getirir
    Task<List<AddressDto>> GetMyAddressesAsync(Guid userId, CancellationToken ct);
}
