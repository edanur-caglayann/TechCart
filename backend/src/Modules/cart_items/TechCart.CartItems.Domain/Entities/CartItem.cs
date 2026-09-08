using TechCart.SharedKernel.Entities;

namespace TechCart.CartItems.Domain.Entities;

public sealed class CartItem : CreationAuditedEntity
{
    public Guid CartId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
}
