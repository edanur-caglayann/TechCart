using TechCart.Addresses.Domain.Entities;

namespace TechCart.Addresses.Domain.Repositories;

public interface IAddressWriteRepository
{
    // Eklenen adres ilk adresse otomatik olarak varsayılan yapılabilmesi için kullanılır.
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken ct);

    // addressId + userId birlikte filtrelenir
    Task<Address?> GetByIdAsync(Guid addressId, Guid userId, CancellationToken ct);

    Task<List<Address>> GetAllByUserIdAsync(Guid userId, CancellationToken ct);

    Task AddAsync(Address address, CancellationToken ct);
    void Remove(Address address);
    Task SaveChangesAsync(CancellationToken ct);
}