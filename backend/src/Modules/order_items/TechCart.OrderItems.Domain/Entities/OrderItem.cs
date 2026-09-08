using TechCart.SharedKernel.Entities;

namespace TechCart.OrderItems.Domain.Entities;

public sealed class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }

    public Guid? ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ProductModel { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal VatRate { get; set; }

    public decimal VatAmount { get; set; }
}
