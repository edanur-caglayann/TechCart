using TechCart.Addresses.Application.Abstractions;
using TechCart.Addresses.Domain.Exceptions;
using TechCart.Addresses.Domain.Repositories;

namespace TechCart.Addresses.Application.SetDefault;

// varsayilan yapilacak adresin AddresId bilgisini ve adres sahibinin UserId bilgisini tasir.
public record SetDefaultAddressCommand(Guid AddressId, Guid UserId);

public class SetDefaultAddressHandler
{
    private readonly IAddressWriteRepository _addressWriteRepository;
    public SetDefaultAddressHandler(IAddressWriteRepository addressWriteRepository) => _addressWriteRepository = addressWriteRepository;

    public async Task<List<AddressDto>> Handle(SetDefaultAddressCommand command, CancellationToken ct)
    {
        // tum adresleri db'den getirir
        var addresses = await _addressWriteRepository.GetAllByUserIdAsync(command.UserId, ct);
        // FirstOrDefault ile varsayilan yapilmak istenen adres listede aranir
        var target = addresses.FirstOrDefault(a => a.Id == command.AddressId)
                     ?? throw new AddressNotFoundException(command.AddressId);

        // "Aynı anda sadece 1 varsayılan olabilir" Secilen adres MarkAsDefault ile varsayilan yapilir
        foreach (var address in addresses)
        {
            if (address.Id == target.Id) address.MarkAsDefault();
            else if (address.IsDefault) address.UnmarkAsDefault();
        }

        await _addressWriteRepository.SaveChangesAsync(ct);

        // Zaten elimizde güncel liste var, tekrar DB'ye gitmeden DTO'ya çeviriyoruz.
        return addresses
            .OrderBy(a => a.CreatedAt)
            .Select(a => new AddressDto(a.Id, a.Title, a.FullName, a.Phone,
                a.City, a.District, a.Neighborhood, a.AddressLine, a.PostalCode, a.IsDefault))
            .ToList();
    }
}