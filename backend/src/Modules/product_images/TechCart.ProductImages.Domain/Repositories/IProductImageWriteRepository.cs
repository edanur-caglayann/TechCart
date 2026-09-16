using TechCart.ProductImages.Domain.Entities;

namespace TechCart.ProductImages.Domain.Repositories;

public interface IProductImageWriteRepository
{
    Task AddAsync(ProductImage image, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}