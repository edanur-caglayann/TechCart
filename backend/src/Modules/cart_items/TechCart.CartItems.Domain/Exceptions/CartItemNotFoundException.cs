using TechCart.SharedKernel;

namespace TechCart.CartItems.Domain.Exceptions;

public class CartItemNotFoundException : AppException
{
    public CartItemNotFoundException(Guid productId)
        : base("CART_ITEM_NOT_FOUND", $"'{productId}' id'li ürün sepette bulunamadı.", statusCode: 404) { }
}