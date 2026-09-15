using Microsoft.EntityFrameworkCore;
using TechCart.Inventory.Domain.Entities;
using TechCart.Inventory.Domain.Repositories;

namespace TechCart.Inventory.Infrastructure.Repositories;

public class ProductStockWriteRepository : IProductStockWriteRepository
{
    private readonly InventoryDbContext _dbContext;
    public ProductStockWriteRepository(InventoryDbContext dbContext) => _dbContext = dbContext;

    public async Task<HashSet<Guid>> GetAllProductIdsAsync(CancellationToken ct)
        => (await _dbContext.ProductStocks.Select(s => s.ProductId).ToListAsync(ct)).ToHashSet();

    public async Task AddAsync(ProductStock stock, CancellationToken ct)
        => await _dbContext.ProductStocks.AddAsync(stock, ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
}