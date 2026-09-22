using TechCart.SharedKernel;

namespace TechCart.Orders.Domain.Exceptions;

public class EmptyCartException : AppException
{
    public EmptyCartException()
        : base("EMPTY_CART", "Sepetiniz boş, sipariş oluşturulamaz.", statusCode: 400) { }
}