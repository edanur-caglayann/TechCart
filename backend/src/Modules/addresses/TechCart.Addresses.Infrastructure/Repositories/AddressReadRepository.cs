using Microsoft.EntityFrameworkCore;
using TechCart.Addresses.Application.Abstractions;

namespace TechCart.Addresses.Infrastructure.Repositories;

public class AddressReadRepository : IAddressReadRepository
{
    private readonly AddressesDbContext _dbContext;
    public AddressReadRepository(AddressesDbContext dbContext) => _dbContext = dbContext;

    public Task<List<AddressDto>> GetMyAddressesAsync(Guid userId, CancellationToken ct)
        => _dbContext.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.CreatedAt)
            .Select(a => new AddressDto(a.Id, a.Title, a.FullName, a.Phone,
                a.City, a.District, a.Neighborhood, a.AddressLine, a.PostalCode, a.IsDefault))
            .ToListAsync(ct);
}