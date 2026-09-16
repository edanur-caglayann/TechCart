namespace TechCart.ProductImages.Application.Abstractions;

public interface IProductImageReadRepository
{
    // Liste ekranı için: birden fazla ürünün ilk görselini tek sorguda getirir
    // sayfa başına 20 ürün varsa 20 ayrı sorgu atmayiz
    Task<Dictionary<Guid, string>> GetPrimaryImagesByProductIdsAsync(IEnumerable<Guid> productIds, CancellationToken ct);

    // Detay ekranı için: tek ürünün tüm görsellerini sırayla getirir.
    Task<List<string>> GetImagesByProductIdAsync(Guid productId, CancellationToken ct);
}