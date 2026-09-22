using TechCart.SharedKernel;

namespace TechCart.CartItems.Domain.Exceptions;

// Sepete eklenmek istenen ürün Products'ta bulunamadı (silinmiş/hiç yok).
public class ProductNotFoundException : AppException
{
    public ProductNotFoundException(Guid productId)
        : base("PRODUCT_NOT_FOUND", $"'{productId}' id'li ürün bulunamadı.", statusCode: 404) { }
}