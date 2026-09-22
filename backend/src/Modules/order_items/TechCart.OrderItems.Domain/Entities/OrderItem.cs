using TechCart.SharedKernel.Entities;

namespace TechCart.OrderItems.Domain.Entities;

public sealed class OrderItem : BaseEntity 
{
    public Guid OrderId { get; private set; }
    public Guid? ProductId { get; private set; } 
    public string ProductName { get; private set; } = string.Empty;
    public string ProductModel { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal VatRate { get; private set; }
    public decimal VatAmount { get; private set; }

    private OrderItem() { } 

    private OrderItem(Guid orderId, Guid? productId, string productName, string productModel,
        int quantity, decimal unitPrice, decimal vatRate, decimal vatAmount)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        ProductModel = productModel;
        Quantity = quantity;
        UnitPrice = unitPrice;
        VatRate = vatRate;
        VatAmount = vatAmount;
    }

    // siparise eklenen bir urun satiri olusturulduktan sonra degistirilemez.
    public static OrderItem Create(Guid orderId, Guid? productId, string productName, string productModel,
        int quantity, decimal unitPrice, decimal vatRate, decimal vatAmount)
        => new(orderId, productId, productName, productModel, quantity, unitPrice, vatRate, vatAmount);
}