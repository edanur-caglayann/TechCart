namespace TechCart.Users.Application.Dtos.ResponseDtos;

public record AuthUserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role);
