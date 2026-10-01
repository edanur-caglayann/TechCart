namespace TechCart.Payments.Application.Dtos.RequestDtos;

public record InitiateCheckoutCommand(
    Guid OrderId,
    Guid UserId,
    string CallbackUrl);