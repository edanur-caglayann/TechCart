using TechCart.Addresses.Application.Dtos.RequestDtos;
using TechCart.Addresses.Application.Dtos.ResponseDtos;
using TechCart.Addresses.Domain.Exceptions;
using TechCart.Addresses.Domain.Repositories;

namespace TechCart.Addresses.Application.Detail;

public class GetAddressDetailHandler(IAddressWriteRepository addressWriteRepository)
{
    // getirilecek adresin AddressId bilgisi ve adres sahibinin UserId bilgisi
    public async Task<AddressDto> Handle(GetAddressDetailQuery query, CancellationToken ct)
    {
        // addressId ve userId ile eslesen adres aranir.
        var address = await addressWriteRepository.GetByIdAsync(query.AddressId, query.UserId, ct)
                      ?? throw new AddressNotFoundException(query.AddressId);

        // bulununa kullaniciya ait adresin detaylari dto tipinde doner
        return new AddressDto(address.Id, address.Title, address.FullName, address.Phone,
            address.City, address.District, address.Neighborhood, address.AddressLine, address.PostalCode, address.IsDefault);
    }
}