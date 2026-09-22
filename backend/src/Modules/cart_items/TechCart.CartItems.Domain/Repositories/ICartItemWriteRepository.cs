using TechCart.CartItems.Domain.Entities;

namespace TechCart.CartItems.Domain.Repositories;

public interface ICartItemWriteRepository
{
    // Sepete ekleme, adet güncelleme, tekil silme gbii islemlerde
    // "bu kullanıcının bu ürünü sepette var mı"  kontrolu icin
    Task<CartItem?> GetByUserAndProductAsync(Guid userId, Guid productId, CancellationToken ct);

    // Birleştirme (merge) ve "sepeti temizle" işlemlerinin ihtiyacı icin
    // kullanıcının tum sepetetini getirirr
    Task<List<CartItem>> GetAllByUserAsync(Guid userId, CancellationToken ct);

    Task AddAsync(CartItem cartItem, CancellationToken ct);
    void Remove(CartItem cartItem);
    void RemoveRange(IEnumerable<CartItem> cartItems);

    Task SaveChangesAsync(CancellationToken ct);
}