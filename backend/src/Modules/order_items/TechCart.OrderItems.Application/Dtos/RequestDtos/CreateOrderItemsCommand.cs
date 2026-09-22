namespace TechCart.OrderItems.Application.Dtos.RequestDtos;

public record OrderItemLine(Guid? ProductId, string ProductName, string ProductModel,
    int Quantity, decimal UnitPrice, decimal VatRate, decimal VatAmount);

// olusturulmus siparisin OrderId bilgisini ve bu siparise eklenecek urunlerin listesini tasir
public record CreateOrderItemsCommand(Guid OrderId, List<OrderItemLine> Items);