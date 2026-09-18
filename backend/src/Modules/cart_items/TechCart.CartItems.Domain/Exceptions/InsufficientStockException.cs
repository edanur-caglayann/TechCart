using TechCart.SharedKernel;

namespace TechCart.CartItems.Domain.Exceptions;

// İstenen toplam adet (mevcut sepet + eklenecek), ürünün gerçek stoğunu aşıyor.
public class InsufficientStockException : AppException
{
    public InsufficientStockException(Guid productId, int availableStock)
        : base("INSUFFICIENT_STOCK", $"'{productId}' id'li ürün için yeterli stok yok. Kalan stok: {availableStock}.", statusCode: 409) { }
}