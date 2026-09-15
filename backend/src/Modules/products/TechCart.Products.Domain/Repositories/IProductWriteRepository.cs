using TechCart.Products.Domain.Entities;

namespace TechCart.Products.Domain.Repositories;

public interface IProductWriteRepository
{

    // Ürün adına göre id eşlemesi — hem "bu ürün zaten var mı" 
    // hem de varsa id'sini tek sorguda verir.
    Task<Dictionary<string, Guid>> GetAllProductIdsByNameAsync(CancellationToken ct);
   
    // rengi bos olanlari filreler
    Task<List<Product>> GetProductsWithEmptyColorAsync(CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    // tum urunleri filtreler. Her bir urun uzerinden SetStock() cagirilir
    Task<List<Product>> GetAllAsync(CancellationToken ct);
}
