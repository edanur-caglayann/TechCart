using Microsoft.EntityFrameworkCore;
using TechCart.Categories.Domain.Entities;
using TechCart.Categories.Domain.Repositories;

namespace TechCart.Categories.Infrastructure.Repositories;

public class CategoryWriteRepository : ICategoryWriteRepository
{
    private readonly CategoriesDbContext _dbContext;
    public CategoryWriteRepository(CategoriesDbContext dbContext) => _dbContext = dbContext;

    public Task<Category?> GetByNameAsync(string name, CancellationToken ct)
        => _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == name, ct);

    public async Task AddAsync(Category category, CancellationToken ct)
        => await _dbContext.Categories.AddAsync(category, ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
}