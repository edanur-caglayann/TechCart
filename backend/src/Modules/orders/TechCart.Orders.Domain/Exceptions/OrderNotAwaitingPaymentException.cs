using TechCart.SharedKernel;

namespace TechCart.Orders.Domain.Exceptions;

public class OrderNotAwaitingPaymentException : AppException
{
    public OrderNotAwaitingPaymentException(Guid orderId, string currentStatus)
        : base("ORDER_NOT_AWAITING_PAYMENT",
            $"'{orderId}' id'li sipariş '{currentStatus}' durumunda, ödeme başlatılamaz.", statusCode: 409) { }
}