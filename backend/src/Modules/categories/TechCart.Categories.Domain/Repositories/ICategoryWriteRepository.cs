using TechCart.Categories.Domain.Entities;

namespace TechCart.Categories.Domain.Repositories;

public interface ICategoryWriteRepository
{
    // Seed script'in "get-or-create" ihtiyacı için
    Task<Category?> GetByNameAsync(string name, CancellationToken ct);

    Task AddAsync(Category category, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}