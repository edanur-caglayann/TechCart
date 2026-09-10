using TechCart.SharedKernel;

namespace TechCart.Addresses.Domain.Exceptions;

// Adres yoksa veya baska bir kullaniciya ait bir adres ise (IDOR korumasi)
public class AddressNotFoundException : AppException
{
    public AddressNotFoundException(Guid addressId)
        : base("ADDRESS_NOT_FOUND", $"'{addressId}' id'li adres bulunamadı.", statusCode: 404) { }
}