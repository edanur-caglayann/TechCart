using TechCart.SharedKernel;

namespace TechCart.Orders.Domain.Exceptions;

public class InsufficientStockException : AppException
{
    public InsufficientStockException(Guid productId, int availableStock)
        : base("INSUFFICIENT_STOCK", $"'{productId}' id'li ürün için yeterli stok yok. Kalan stok: {availableStock}.", statusCode: 409) { }
}