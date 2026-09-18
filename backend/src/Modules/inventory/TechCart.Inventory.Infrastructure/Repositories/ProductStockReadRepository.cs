using Microsoft.EntityFrameworkCore;
using TechCart.Inventory.Application.Abstractions;
using TechCart.Inventory.Application.Dtos.ResponseDtos;

namespace TechCart.Inventory.Infrastructure.Repositories;

public class ProductStockReadRepository : IProductStockReadRepository
{
    private readonly InventoryDbContext _dbContext;
    public ProductStockReadRepository(InventoryDbContext dbContext) => _dbContext = dbContext;

    public Task<ProductStockDto?> GetByProductIdAsync(Guid productId, CancellationToken ct)
        => _dbContext.ProductStocks
            .AsNoTracking()
            .Where(s => s.ProductId == productId)
            .Select(s => new ProductStockDto(s.Stock, s.Stock > 0, s.IsReadyToShip, s.HasFastDelivery))
            .FirstOrDefaultAsync(ct);

    public async Task<Dictionary<Guid, bool>> GetInStockMapAsync(IEnumerable<Guid> productIds, CancellationToken ct)
    {
        var ids = productIds.ToList();

        return await _dbContext.ProductStocks
            .AsNoTracking()
            .Where(s => ids.Contains(s.ProductId))
            .Select(s => new { s.ProductId, InStock = s.Stock > 0 })
            .ToDictionaryAsync(x => x.ProductId, x => x.InStock, ct);
    }
    public async Task<HashSet<Guid>> GetInStockProductIdsAsync(CancellationToken ct)
    {
        var ids = await _dbContext.ProductStocks
            .AsNoTracking()
            .Where(s => s.Stock > 0)
            .Select(s => s.ProductId)
            .ToListAsync(ct);

        return ids.ToHashSet();
    }
    
    public Task<Dictionary<Guid, int>> GetAllStocksAsync(CancellationToken ct)
        => _dbContext.ProductStocks
            .AsNoTracking()
            .ToDictionaryAsync(s => s.ProductId, s => s.Stock, ct);
    
}
