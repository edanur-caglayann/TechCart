using Microsoft.EntityFrameworkCore;
using TechCart.Brands.Application.Abstractions;
using TechCart.Brands.Application.ResponseDtos;

namespace TechCart.Brands.Infrastructure.Repositories;

public class BrandReadRepository : IBrandReadRepository
{
    private readonly BrandsDbContext _dbContext;
    public BrandReadRepository(BrandsDbContext dbContext) => _dbContext = dbContext;

    public Task<List<BrandResponse>> GetAllAsync(CancellationToken ct)
        => _dbContext.Brands
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new BrandResponse(b.Id, b.Name))
            .ToListAsync(ct);
    public Task<List<BrandResponse>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
        => _dbContext.Brands
            .AsNoTracking()
            .Where(c => ids.Contains(c.Id))
            .Select(c => new BrandResponse(c.Id, c.Name))
            .ToListAsync(ct);

    public Task<List<string>> SearchByNameAsync(string term, int limit, CancellationToken ct)
        => _dbContext.Brands
            .AsNoTracking()
            .Where(c => EF.Functions.ILike(c.Name, $"%{term}%"))
            .OrderBy(c => c.Name)
            .Select(c => c.Name)
            .Take(limit)
            .ToListAsync(ct);
}
