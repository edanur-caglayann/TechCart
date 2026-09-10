using TechCart.Addresses.Application.Abstractions;
using TechCart.Addresses.Domain.Entities;
using TechCart.Addresses.Domain.Repositories;

namespace TechCart.Addresses.Application.Create;

// kullanicidan gelen yeni adres bilgisini tasir
public record CreateAddressCommand(Guid UserId, string Title, string FullName, string Phone,
    string City, string District,string Neighborhood, string AddressLine, string PostalCode);

public class CreateAddressHandler
{
    private readonly IAddressWriteRepository _addressWriteRepository;
    public CreateAddressHandler(IAddressWriteRepository addressWriteRepository) => _addressWriteRepository = addressWriteRepository;

    public async Task<AddressDto> Handle(CreateAddressCommand command, CancellationToken ct)
    {
        // kullanıcının ilk adresiyse otomatik varsayılan.
        // CountByUserIdAsync() ile kullanicinin mevcut adres sayisi ogrenilir hic adres yoksa isFirstAddress degeri true olur
        var existingCount = await _addressWriteRepository.CountByUserIdAsync(command.UserId, ct);
        var isFirstAddress = existingCount == 0;

        var address = Address.Create(command.UserId, command.Title, command.FullName, command.Phone,
            command.City, command.District, command.Neighborhood, command.AddressLine, command.PostalCode, isDefault: isFirstAddress);

        await _addressWriteRepository.AddAsync(address, ct);
        await _addressWriteRepository.SaveChangesAsync(ct);

        return new AddressDto(address.Id, address.Title, address.FullName, address.Phone,
            address.City, address.District, address.Neighborhood, address.AddressLine, address.PostalCode, address.IsDefault);
    }
}