using TechCart.Inventory.Domain.Entities;

namespace TechCart.Inventory.Domain.Repositories;

public interface IProductStockWriteRepository
{
    // Seeder'ın "bu ürünün zaten stok kaydı var mı" kontrolü için
    // tek sorguyla tum stoklu ürün id'lerini belleğe çeker 
    Task<HashSet<Guid>> GetAllProductIdsAsync(CancellationToken ct);

    Task AddAsync(ProductStock stock, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    
    // true = rezervasyon başarılı (satır güncellendi),
    // false = yeterli müsait stok yoktu 
    Task<bool> TryReserveAsync(Guid productId, int quantity, CancellationToken ct);

    // Ödeme başarılı: rezervasyonu kalıcı düşüşe çevirir. 
    Task<int> ConfirmReservationAsync(Guid productId, int quantity, CancellationToken ct);
    
    // Ödeme başarısız / rezervasyon süresi doldu: sadece rezervasyonu geri
    // alır, Stock'a hiç dokunmaz.
    Task ReleaseReservationAsync(Guid productId, int quantity, CancellationToken ct);

}