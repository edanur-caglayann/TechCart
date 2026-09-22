namespace TechCart.Payments.Application.Dtos.RequestDtos;

public record ProcessPaymentCallbackCommand(string Token, Guid OrderId);