using TechCart.Addresses.Application.Abstractions;
using TechCart.Addresses.Application.Dtos.RequestDtos;
using TechCart.Addresses.Application.Dtos.ResponseDtos;

namespace TechCart.Addresses.Application.List;

// Query'yi karsilar, kullanici id'sini repository'ye gondererek adres listesini alir
public class ListMyAddressesHandler
{
    private readonly IAddressReadRepository _addressReadRepository;
    public ListMyAddressesHandler(IAddressReadRepository addressReadRepository) => _addressReadRepository = addressReadRepository;

    public Task<List<AddressDto>> Handle(ListMyAddressesQuery query, CancellationToken ct)
        => _addressReadRepository.GetMyAddressesAsync(query.UserId, ct);
}
