using TechCart.SharedKernel.Entities;

namespace TechCart.Payments.Domain.Entities;

public sealed class Payment : AuditableEntity
{
    public Guid OrderId { get; set; }

    public string Provider { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = string.Empty;

    public string ProviderReference { get; set; } = string.Empty;

    public string? TdsReference { get; set; }

    public string? FailureReason { get; set; }
}
