using TechCart.Brands.Domain.Entities;

namespace TechCart.Brands.Domain.Repositories;

public interface IBrandWriteRepository
{
    Task<Brand?> GetByNameAsync(string name, CancellationToken ct);

    Task AddAsync(Brand brand, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}