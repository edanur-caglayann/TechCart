using TechCart.SharedKernel.Entities;

namespace TechCart.Orders.Domain.Entities;

public sealed class Order : AuditableEntity
{
    public Guid UserId { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal Subtotal { get; set; }

    public decimal VatTotal { get; set; }

    public decimal ShippingFee { get; set; }

    public decimal Total { get; set; }

    public string ShippingFullName { get; set; } = string.Empty;

    public string ShippingPhone { get; set; } = string.Empty;

    public string ShippingCity { get; set; } = string.Empty;

    public string ShippingDistrict { get; set; } = string.Empty;

    public string ShippingAddressLine { get; set; } = string.Empty;

    public string ShippingPostalCode { get; set; } = string.Empty;
}
