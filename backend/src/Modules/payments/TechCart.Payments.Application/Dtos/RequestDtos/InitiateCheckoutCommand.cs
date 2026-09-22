namespace TechCart.Payments.Application.Dtos.RequestDtos;

// hangi siparis, hangi kullanici 
public record InitiateCheckoutCommand(Guid OrderId, Guid UserId);