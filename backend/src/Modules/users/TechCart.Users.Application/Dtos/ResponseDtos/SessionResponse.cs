namespace TechCart.Users.Application.Dtos.ResponseDtos;

public record SessionResponse(bool IsAuthenticated, AuthUserResponse User);
