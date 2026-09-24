using TechCart.OrderItems.Application.Dtos.RequestDtos;
using TechCart.OrderItems.Application.List;
using TechCart.Orders.Application.Dtos.RequestDtos;
using TechCart.Orders.Contracts.ResponseDtos;
using TechCart.Orders.Domain.Repositories;
using TechCart.Payments.Application.Dtos.RequestDtos;
using TechCart.Payments.Application.List;

namespace TechCart.Orders.Application.ListMyOrders;

public class ListMyOrdersHandler(
    IOrderWriteRepository orderWriteRepository,
    ListOrderItemsHandler listOrderItemsHandler,
    ListPaymentSummariesHandler listPaymentSummariesHandler)
{
    public async Task<List<OrderResponse>> Handle(ListMyOrdersQuery query, CancellationToken ct)
    {
        var orders = await orderWriteRepository.GetByUserIdAsync(query.UserId, ct);

        if (orders.Count == 0)
            return [];

        var orderIds = orders.Select(o => o.Id).ToList();

        // Ödeme özetlerini toplu çekiyoruz 
        var paymentSummaries = await listPaymentSummariesHandler.Handle(
            new ListPaymentSummariesQuery(orderIds), ct);
        var paymentsByOrderId = paymentSummaries.ToDictionary(p => p.OrderId);

        var result = new List<OrderResponse>();

        foreach (var order in orders)
        {
            var items = await listOrderItemsHandler.Handle(new ListOrderItemsQuery(order.Id), ct);

            paymentsByOrderId.TryGetValue(order.Id, out var payment);

            result.Add(new OrderResponse(
                order.Id, order.OrderNumber, order.Status.ToString(), order.CreatedAt,
                order.Subtotal, order.VatTotal, order.ShippingFee, order.Total,
                order.ShippingFullName, order.ShippingPhone, order.ShippingCity,
                order.ShippingDistrict, order.ShippingAddressLine, order.ShippingPostalCode,
                payment is null ? null : new OrderPaymentSummaryResponse(payment.Provider, payment.MaskedCardNumber, payment.PaidAt),
                items.Select(item => new OrderItemLineResponse(
                    item.ProductId, item.ProductName, item.ProductModel, item.Quantity, item.UnitPrice, item.VatRate, item.VatAmount)).ToList()));
        }

        return result;
    }
}