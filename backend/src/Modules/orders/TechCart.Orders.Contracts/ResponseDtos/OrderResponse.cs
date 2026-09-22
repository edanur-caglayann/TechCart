namespace TechCart.Orders.Contracts.ResponseDtos;

public record OrderResponse(
    Guid Id,
    string OrderNumber,
    string Status,
    decimal Subtotal,
    decimal VatTotal,
    decimal ShippingFee,
    decimal Total,
    List<OrderItemLineResponse> Items);