using TechCart.Payments.Application.Dtos.ResponseDtos;
using TechCart.Payments.Contracts.ResponseDtos;

namespace TechCart.Payments.Application.Abstractions;

public interface IPaymentGateway
{
    Task<CheckoutInitResponse> InitializeCheckoutAsync(
        string conversationId, decimal price, string buyerName, string buyerSurname,
        string buyerEmail, string buyerPhone, string buyerAddress, string buyerCity,
        string callbackUrl, List<(string Name, decimal Price)> basketItems, CancellationToken ct);
    
    Task<PaymentResultDto> RetrieveCheckoutResultAsync(string token, CancellationToken ct);
}