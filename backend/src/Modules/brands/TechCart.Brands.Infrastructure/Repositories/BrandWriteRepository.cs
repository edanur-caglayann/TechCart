using Microsoft.EntityFrameworkCore;
using TechCart.Brands.Domain.Entities;
using TechCart.Brands.Domain.Repositories;

namespace TechCart.Brands.Infrastructure.Repositories;

public class BrandWriteRepository : IBrandWriteRepository
{
    private readonly BrandsDbContext _dbContext;
    public BrandWriteRepository(BrandsDbContext dbContext) => _dbContext = dbContext;

    public Task<Brand?> GetByNameAsync(string name, CancellationToken ct)
        => _dbContext.Brands.FirstOrDefaultAsync(b => b.Name == name, ct);

    public async Task AddAsync(Brand brand, CancellationToken ct)
        => await _dbContext.Brands.AddAsync(brand, ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
}