namespace TechCart.Orders.Contracts.ResponseDtos;

public record OrderResponse(
    Guid Id,
    string OrderNumber,
    string Status,
    DateTime CreatedAt,
    decimal Subtotal,
    decimal VatTotal,
    decimal ShippingFee,
    decimal Total,
    string ShippingFullName,
    string ShippingPhone,
    string ShippingCity,
    string ShippingDistrict,
    string ShippingAddressLine,
    string ShippingPostalCode,
    OrderPaymentSummaryResponse? Payment, // null = henüz ödenmemiş
    List<OrderItemLineResponse> Items);