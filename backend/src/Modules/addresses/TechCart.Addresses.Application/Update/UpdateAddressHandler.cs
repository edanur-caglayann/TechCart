using TechCart.Addresses.Application.Abstractions;
using TechCart.Addresses.Domain.Exceptions;
using TechCart.Addresses.Domain.Repositories;

namespace TechCart.Addresses.Application.Update;

public record UpdateAddressCommand(Guid AddressId, Guid UserId, string Title, string FullName,
    string Phone, string City, string District,string Neighborhood, string AddressLine, string PostalCode);

public class UpdateAddressHandler
{
    private readonly IAddressWriteRepository _addressWriteRepository;
    public UpdateAddressHandler(IAddressWriteRepository addressWriteRepository) => _addressWriteRepository = addressWriteRepository;

    public async Task<AddressDto> Handle(UpdateAddressCommand command, CancellationToken ct)
    {
        // adres aranir
        var address = await _addressWriteRepository.GetByIdAsync(command.AddressId, command.UserId, ct)
                      ?? throw new AddressNotFoundException(command.AddressId);

        address.UpdateDetails(command.Title, command.FullName, command.Phone,
            command.City, command.District, command.Neighborhood, command.AddressLine, command.PostalCode);
        await _addressWriteRepository.SaveChangesAsync(ct);

        return new AddressDto(address.Id, address.Title, address.FullName, address.Phone,
            address.City, address.District, address.Neighborhood, address.AddressLine, address.PostalCode, address.IsDefault);
    }
}