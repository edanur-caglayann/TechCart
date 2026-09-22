using TechCart.SharedKernel;

namespace TechCart.CartItems.Domain.Exceptions;
public class InvalidQuantityException : AppException
{
    public InvalidQuantityException()
        : base("INVALID_QUANTITY", "Adet en az 1 olmalıdır.", statusCode: 400) { }
}