using TechCart.Orders.Domain.Enums;
using TechCart.SharedKernel.Entities;

namespace TechCart.Orders.Domain.Entities;

public sealed class Order : AuditableEntity
{
    public Guid UserId { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal VatTotal { get; private set; }
    public decimal ShippingFee { get; private set; }
    public decimal Total { get; private set; }

    public string ShippingFullName { get; private set; } = string.Empty;
    public string ShippingPhone { get; private set; } = string.Empty;
    public string ShippingCity { get; private set; } = string.Empty;
    public string ShippingDistrict { get; private set; } = string.Empty;
    public string ShippingAddressLine { get; private set; } = string.Empty;
    public string ShippingPostalCode { get; private set; } = string.Empty;

    private Order() { } 

    private Order(Guid userId, string orderNumber, decimal subtotal, decimal vatTotal, decimal shippingFee, decimal total,
        string shippingFullName, string shippingPhone, string shippingCity, string shippingDistrict,
        string shippingAddressLine, string shippingPostalCode)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        OrderNumber = orderNumber;
        Status = OrderStatus.AwaitingPayment; // her sipariş hep bu durumda doğar
        Subtotal = subtotal;
        VatTotal = vatTotal;
        ShippingFee = shippingFee;
        Total = total;
        ShippingFullName = shippingFullName;
        ShippingPhone = shippingPhone;
        ShippingCity = shippingCity;
        ShippingDistrict = shippingDistrict;
        ShippingAddressLine = shippingAddressLine;
        ShippingPostalCode = shippingPostalCode;
        CreatedAt = DateTime.UtcNow;
    }
    
    public static Order Create(Guid userId, string orderNumber, decimal subtotal, decimal vatTotal,
        decimal shippingFee, decimal total, string shippingFullName, string shippingPhone, string shippingCity,
        string shippingDistrict, string shippingAddressLine, string shippingPostalCode)
        => new(userId, orderNumber, subtotal, vatTotal, shippingFee, total,
            shippingFullName, shippingPhone, shippingCity, shippingDistrict, shippingAddressLine, shippingPostalCode);

    // Bu metot, tracked (izlenen) entity ile normal SaveChanges akışında
    // çalışan senaryolar için (örn. Worker'ın süresi dolan siparişi iptal etmesi).
    public void MarkAsPaid()
    {
        if (Status != OrderStatus.AwaitingPayment)
            throw new InvalidOperationException($"Sipariş '{Status}' durumundayken ödendi olarak işaretlenemez.");

        Status = OrderStatus.Paid;
        MarkUpdated();
    }

    public void Cancel()
    {
        if (Status != OrderStatus.AwaitingPayment)
            throw new InvalidOperationException($"Sipariş '{Status}' durumundayken iptal edilemez.");

        Status = OrderStatus.Cancelled;
        MarkUpdated();
    }
}