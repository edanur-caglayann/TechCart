namespace TechCart.Users.Application.Dtos.RequestDtos;

public record LogoutCommand(string Jti, Guid UserId, DateTime ExpiresAt);
