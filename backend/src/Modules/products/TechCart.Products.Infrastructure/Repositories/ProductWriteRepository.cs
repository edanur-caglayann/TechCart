using Microsoft.EntityFrameworkCore;
using TechCart.Products.Domain.Entities;
using TechCart.Products.Domain.Repositories;

namespace TechCart.Products.Infrastructure.Repositories;

public class ProductWriteRepository : IProductWriteRepository
{
    private readonly ProductsDbContext _dbContext;
    public ProductWriteRepository(ProductsDbContext dbContext) => _dbContext = dbContext;

    public async Task<HashSet<string>> GetAllNamesAsync(CancellationToken ct)
    {
        var names = await _dbContext.Products.Select(p => p.Name).ToListAsync(ct);
        // OrdinalIgnoreCase: CSV'deki isim büyük/küçük harf farkıyla gelse bile
        // aynı ürünü ikinci kez eklemeyi engellesin diye.
        return new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
    }
    
    public Task<List<Product>> GetProductsWithEmptyColorAsync(CancellationToken ct)
        => _dbContext.Products.Where(p => p.Color == "").ToListAsync(ct);
    

    public async Task<Dictionary<string, Guid>> GetAllProductIdsByNameAsync(CancellationToken ct)
    {
        var rows = await _dbContext.Products.Select(p => new { p.Name, p.Id }).ToListAsync(ct);
        return rows.ToDictionary(r => r.Name, r => r.Id, StringComparer.OrdinalIgnoreCase);
    }

    public async Task AddAsync(Product product, CancellationToken ct)
        => await _dbContext.Products.AddAsync(product, ct);
    
    public Task<List<Product>> GetAllAsync(CancellationToken ct)
        => _dbContext.Products.ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
}