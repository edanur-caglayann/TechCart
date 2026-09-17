namespace TechCart.Users.Application.Auth.RequestDtos;

public record LogoutCommand(string Jti, Guid UserId, DateTime ExpiresAt);
