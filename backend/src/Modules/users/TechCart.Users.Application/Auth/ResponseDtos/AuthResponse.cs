namespace TechCart.Users.Application.Auth.ResponseDtos;

public record AuthResponse(string Token, AuthUserResponse User);
