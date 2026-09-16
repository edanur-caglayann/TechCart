using TechCart.ProductImages.Domain.Entities;
using TechCart.ProductImages.Domain.Repositories;

namespace TechCart.ProductImages.Infrastructure.Repositories;

public class ProductImageWriteRepository : IProductImageWriteRepository
{
    private readonly ProductImagesDbContext _dbContext;
    public ProductImageWriteRepository(ProductImagesDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(ProductImage image, CancellationToken ct)
        => await _dbContext.ProductImages.AddAsync(image, ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
}