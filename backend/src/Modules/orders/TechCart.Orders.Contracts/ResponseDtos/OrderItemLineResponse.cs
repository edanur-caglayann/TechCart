namespace TechCart.Orders.Contracts.ResponseDtos;

public record OrderItemLineResponse(
    Guid? ProductId,
    string ProductName,
    string ProductModel,
    int Quantity,
    decimal UnitPrice,
    decimal VatRate,
    decimal VatAmount);