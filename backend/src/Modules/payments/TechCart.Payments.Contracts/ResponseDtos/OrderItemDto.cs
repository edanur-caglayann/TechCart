namespace TechCart.OrderItems.Contracts.ResponseDtos;

public record OrderItemDto(Guid? ProductId, string ProductName, string ProductModel,
    int Quantity, decimal UnitPrice, decimal VatRate, decimal VatAmount);