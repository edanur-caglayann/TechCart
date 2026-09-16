namespace TechCart.Users.Application.Profile.ResponseDtos;

public record UserProfileResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    DateTime CreatedAt);
