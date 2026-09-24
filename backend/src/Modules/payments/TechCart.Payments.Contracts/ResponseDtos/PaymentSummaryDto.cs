namespace TechCart.Payments.Contracts.ResponseDtos;

public record PaymentSummaryDto(
    Guid OrderId,
    string Provider,
    string? MaskedCardNumber,
    decimal Amount,
    DateTime PaidAt);