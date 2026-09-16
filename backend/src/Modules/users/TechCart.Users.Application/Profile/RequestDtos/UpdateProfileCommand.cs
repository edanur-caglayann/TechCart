namespace TechCart.Users.Application.Profile.RequestDtos;

public record UpdateProfileCommand(Guid UserId, string FirstName, string LastName, string Email);
