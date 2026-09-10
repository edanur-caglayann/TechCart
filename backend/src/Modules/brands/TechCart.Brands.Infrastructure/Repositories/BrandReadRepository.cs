using Microsoft.EntityFrameworkCore;
using TechCart.Brands.Application.Abstractions;

namespace TechCart.Brands.Infrastructure.Repositories;

public class BrandReadRepository : IBrandReadRepository
{
    private readonly BrandsDbContext _dbContext;
    public BrandReadRepository(BrandsDbContext dbContext) => _dbContext = dbContext;

    public Task<List<BrandDto>> GetAllAsync(CancellationToken ct)
        => _dbContext.Brands
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new BrandDto(b.Id, b.Name))
            .ToListAsync(ct);
}