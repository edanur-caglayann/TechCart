using Microsoft.EntityFrameworkCore;
using TechCart.Addresses.Domain.Entities;
using TechCart.Addresses.Domain.Repositories;

namespace TechCart.Addresses.Infrastructure.Repositories;

public class AddressWriteRepository : IAddressWriteRepository
{
    private readonly AddressesDbContext _dbContext;
    public AddressWriteRepository(AddressesDbContext dbContext) => _dbContext = dbContext;

    public Task<int> CountByUserIdAsync(Guid userId, CancellationToken ct)
        => _dbContext.Addresses.CountAsync(a => a.UserId == userId, ct);

    public Task<Address?> GetByIdAsync(Guid addressId, Guid userId, CancellationToken ct)
        => _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId, ct);

    public Task<List<Address>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
        => _dbContext.Addresses.Where(a => a.UserId == userId).ToListAsync(ct);

    public async Task AddAsync(Address address, CancellationToken ct)
        => await _dbContext.Addresses.AddAsync(address, ct);

    public void Remove(Address address)
        => _dbContext.Addresses.Remove(address);

    public Task SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
}