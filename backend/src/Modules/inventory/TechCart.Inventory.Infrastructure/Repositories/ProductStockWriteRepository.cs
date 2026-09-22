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
    
    
    // Normalde kayit veritabanindan okunur islemler yapilir ve savechanges ile tekrar db'ye kaydedilir
    // Burada ExecuteUpdateAsync ile db'yi dogrudan guncelleriz.
    
    // rezervasyon metotdunda EF Core asagidaki SQL sorgusunu uretir
    // UPDATE product_stocks
    // SET reserved_stock = reserved_stock + @quantity
    // WHERE product_id = @productId
    // AND stock - reserved_stock >= @quantity;
    // boylece db ayni anda hem yeterli stok var mi kontorlu yapar hem de stok yeterliysa 
    // rezerve eder.
    

    // siparis olusturulurken istenen miktari gecici olarak rezerve etmeye calisir.
    public async Task<bool> TryReserveAsync(Guid productId, int quantity, CancellationToken ct)
    {
        var affectedRows = await _dbContext.ProductStocks
            .Where(s => s.ProductId == productId && s.Stock - s.ReservedStock >= quantity)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.ReservedStock, s => s.ReservedStock + quantity), ct);

        return affectedRows > 0; // stok yeterliydi, rezervasyon yapildi
        //  affectedRows == 0 → Ürün bulunamadı veya yeterli stok yoktu.
        
    }
    // odme basariliysa rezervasyondaki urun adedidini stoktan dusurur
    public async Task<int> ConfirmReservationAsync(Guid productId, int quantity, CancellationToken ct)
    {
        await _dbContext.ProductStocks
            .Where(s => s.ProductId == productId && s.ReservedStock >= quantity)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.Stock, s => s.Stock - quantity)
                .SetProperty(s => s.ReservedStock, s => s.ReservedStock - quantity), ct);

        // ExecuteUpdateAsync güncel değeri geri vermiyor, bu yüzden
        // güncellemeden sonra ayrı bir okuma yapıyoruz.
        var stock = await _dbContext.ProductStocks
            .AsNoTracking()
            .Where(s => s.ProductId == productId)
            .Select(s => s.Stock)
            .FirstOrDefaultAsync(ct);

        return stock;
    }

    // basarisiz surecse rezervasyonu serbest birakir.
    public async Task ReleaseReservationAsync(Guid productId, int quantity, CancellationToken ct)
    {
        await _dbContext.ProductStocks
            .Where(s => s.ProductId == productId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.ReservedStock, s => Math.Max(0, s.ReservedStock - quantity)), ct);    }
}