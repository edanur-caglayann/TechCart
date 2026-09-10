using TechCart.Addresses.Application.Abstractions;

namespace TechCart.Addresses.Application.List;

// adresleri listelenecek olan kullanicinin kimligini tasir
public record ListMyAddressesQuery(Guid UserId);

// Query'yi karsilar, kullanici id'sini repository'ye gondererek adres listesini alir
public class ListMyAddressesHandler
{
    private readonly IAddressReadRepository _addressReadRepository;
    public ListMyAddressesHandler(IAddressReadRepository addressReadRepository) => _addressReadRepository = addressReadRepository;

    public Task<List<AddressDto>> Handle(ListMyAddressesQuery query, CancellationToken ct)
        => _addressReadRepository.GetMyAddressesAsync(query.UserId, ct);
}