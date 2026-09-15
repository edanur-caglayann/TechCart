using TechCart.Inventory.Domain.Entities;

namespace TechCart.Inventory.Domain.Repositories;

public interface IProductStockWriteRepository
{
    // Seeder'ın "bu ürünün zaten stok kaydı var mı" kontrolü için
    // tek sorguyla tum stoklu ürün id'lerini belleğe çeker 
    Task<HashSet<Guid>> GetAllProductIdsAsync(CancellationToken ct);

    Task AddAsync(ProductStock stock, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}