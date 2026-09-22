using TechCart.Inventory.Application.Dtos.ResponseDtos;

namespace TechCart.Inventory.Application.Abstractions;

public interface IProductStockReadRepository
{
    // Detay ekranı için: tek ürünün stok bilgisi.
    Task<ProductStockDto?> GetByProductIdAsync(Guid productId, CancellationToken ct);

    // Liste ekranı için: birden fazla ürünün inStock bilgisini tek sorguda getirir.
    Task<Dictionary<Guid, bool>> GetInStockMapAsync(IEnumerable<Guid> productIds, CancellationToken ct);
    
    Task<HashSet<Guid>> GetInStockProductIdsAsync(CancellationToken ct);
    
    // Her urunun stok sayisini dondurur
    Task<Dictionary<Guid, int>> GetAllStocksAsync(CancellationToken ct);
}
