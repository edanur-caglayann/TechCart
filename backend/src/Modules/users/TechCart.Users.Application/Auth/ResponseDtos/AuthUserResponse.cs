namespace TechCart.Users.Application.Auth.ResponseDtos;

public record AuthUserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role);
