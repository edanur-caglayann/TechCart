namespace TechCart.Users.Application.Auth.ResponseDtos;

public record SessionResponse(bool IsAuthenticated, AuthUserResponse User);
