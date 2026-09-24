namespace TechCart.Orders.Contracts.ResponseDtos;

public record OrderPaymentSummaryResponse(
    string Provider,
    string? MaskedCardNumber,
    DateTime PaidAt);