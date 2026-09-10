using TechCart.Addresses.Domain.Exceptions;
using TechCart.Addresses.Domain.Repositories;

namespace TechCart.Addresses.Application.Delete;

public record DeleteAddressCommand(Guid AddressId, Guid UserId);

public class DeleteAddressHandler
{
    private readonly IAddressWriteRepository _addressWriteRepository;
    public DeleteAddressHandler(IAddressWriteRepository addressWriteRepository) => _addressWriteRepository = addressWriteRepository;

    public async Task Handle(DeleteAddressCommand command, CancellationToken ct)
    {
        // Tum adresleri tek sorguda cekeriz. Hem silinecek olani hem yeni varsayilan adres karari
        var addresses = await _addressWriteRepository.GetAllByUserIdAsync(command.UserId, ct);
        var addressToDelete = addresses.FirstOrDefault(a => a.Id == command.AddressId)
                              ?? throw new AddressNotFoundException(command.AddressId);

        var wasDefault = addressToDelete.IsDefault;
        _addressWriteRepository.Remove(addressToDelete);

        if (wasDefault)
        {
            // silinen adres varsayilan adres ise ilk eklenen yani en eski adresi varsayilan seceriz
            var newDefault = addresses
                .Where(a => a.Id != command.AddressId)
                .OrderBy(a => a.CreatedAt)
                .FirstOrDefault();

            newDefault?.MarkAsDefault();
        }

        await _addressWriteRepository.SaveChangesAsync(ct);
    }
}