using TechCart.SharedKernel.Entities;

namespace TechCart.Carts.Domain.Entities;

public sealed class Cart : AuditableEntity
{
    public Guid? UserId { get; set; }

    public string? GuestToken { get; set; }
}
