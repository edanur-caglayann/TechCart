namespace TechCart.SharedKernel.Entities;

public abstract class CreationAuditedEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
