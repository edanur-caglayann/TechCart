using TechCart.SharedKernel;

namespace TechCart.Products.Domain.Exceptions;

public class ProductNotFoundException : AppException
{
    public ProductNotFoundException(Guid productId)
        : base("PRODUCT_NOT_FOUND", $"'{productId}' id'li ürün bulunamadı.", statusCode: 404) { }
}