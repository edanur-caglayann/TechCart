namespace TechCart.Inventory.Application.Abstractions;

public record ProductStockDto(int Stock, bool InStock, bool IsReadyToShip, bool HasFastDelivery);

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