using TechCart.Products.Domain.Entities;

namespace TechCart.Products.Domain.Repositories;

public interface IProductWriteRepository
{
    // tüm isimleri TEK sorguyla belleğe çekip döngü içinde ona bakacağız —
    Task<HashSet<string>> GetAllNamesAsync(CancellationToken ct);

    Task AddAsync(Product product, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}