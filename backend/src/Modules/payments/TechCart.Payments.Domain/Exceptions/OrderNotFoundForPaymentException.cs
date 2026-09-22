using TechCart.SharedKernel;

namespace TechCart.Payments.Domain.Exceptions;

public class OrderNotFoundForPaymentException : AppException
{
    public OrderNotFoundForPaymentException(string conversationId)
        : base("ORDER_NOT_FOUND_FOR_PAYMENT",
            $"'{conversationId}' conversationId'sine ait sipariş bulunamadı.", statusCode: 404) { }
}