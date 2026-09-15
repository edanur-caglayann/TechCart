using Microsoft.EntityFrameworkCore;
using TechCart.ProductImages.Application.Abstractions;

namespace TechCart.ProductImages.Infrastructure.Repositories;

public class ProductImageReadRepository : IProductImageReadRepository
{
    private readonly ProductImagesDbContext _dbContext;
    public ProductImageReadRepository(ProductImagesDbContext dbContext) => _dbContext = dbContext;

    public async Task<Dictionary<Guid, string>> GetPrimaryImagesByProductIdsAsync(IEnumerable<Guid> productIds, CancellationToken ct)
    {
        var ids = productIds.ToList();

        return await _dbContext.ProductImages
            .AsNoTracking()
            .Where(i => ids.Contains(i.ProductId))
            .GroupBy(i => i.ProductId)
            // Her ürün grubundan sort_order'ı en küçük olan görseli seçiyoruz.
            .Select(g => new { ProductId = g.Key, ImageUrl = g.OrderBy(i => i.SortOrder).First().ImageUrl })
            .ToDictionaryAsync(x => x.ProductId, x => x.ImageUrl, ct);
    }

    public Task<List<string>> GetImagesByProductIdAsync(Guid productId, CancellationToken ct)
        => _dbContext.ProductImages
            .AsNoTracking()
            .Where(i => i.ProductId == productId)
            .OrderBy(i => i.SortOrder)
            .Select(i => i.ImageUrl)
            .ToListAsync(ct);
}