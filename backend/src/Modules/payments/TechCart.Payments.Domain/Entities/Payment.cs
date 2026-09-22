using TechCart.Payments.Domain.Enums;
using TechCart.SharedKernel.Entities;

namespace TechCart.Payments.Domain.Entities;

public sealed class Payment : AuditableEntity
{
    public Guid OrderId { get; private set; }
    public string Provider { get; private set; } = string.Empty;
    public PaymentStatus Status { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string ProviderReference { get; private set; } = string.Empty;
    public string? TdsReference { get; private set; }
    public string? FailureReason { get; private set; }

    private Payment() { } 

    private Payment(Guid orderId, string provider, PaymentStatus status, decimal amount, string currency,
        string providerReference, string? tdsReference, string? failureReason)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Provider = provider;
        Status = status;
        Amount = amount;
        Currency = currency;
        ProviderReference = providerReference; // ayni iyzcio ilemi icin 2 kez payment satiri olusmamsi icin 
        TdsReference = tdsReference;
        FailureReason = failureReason;
        CreatedAt = DateTime.UtcNow;
    }

    // Her odeme denemesi icin ayri bir Payment kaydi tutulur.
    // Ilk kartla odeme basarisiz oldu -> CreateFailed cagrilir ve basarisiz bir paymnet kaydi olsuturulur
    // Kullanici baska bir kartla yeniden dener. ikinci odeme basarili oldu. -> CreateSucceeded cagrilir ve
    // yeni bir payment satiri olusturulur.
    public static Payment CreateSucceeded(Guid orderId, string provider, decimal amount, string currency,
        string providerReference, string? tdsReference)
        => new(orderId, provider, PaymentStatus.Succeeded, amount, currency, providerReference, tdsReference, failureReason: null);

    public static Payment CreateFailed(Guid orderId, string provider, decimal amount, string currency,
        string providerReference, string failureReason)
        => new(orderId, provider, PaymentStatus.Failed, amount, currency, providerReference, tdsReference: null, failureReason);
}