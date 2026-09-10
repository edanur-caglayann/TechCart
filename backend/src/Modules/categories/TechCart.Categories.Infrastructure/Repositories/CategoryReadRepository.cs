using Microsoft.EntityFrameworkCore;
using TechCart.Categories.Application.Abstractions;

namespace TechCart.Categories.Infrastructure.Repositories;

public class CategoryReadRepository : ICategoryReadRepository
{
    private readonly CategoriesDbContext _dbContext;
    public CategoryReadRepository(CategoriesDbContext dbContext) => _dbContext = dbContext;

    public Task<List<CategoryDto>> GetAllAsync(CancellationToken ct)
        => _dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToListAsync(ct);
}