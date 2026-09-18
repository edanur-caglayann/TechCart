namespace TechCart.Users.Application.Dtos.RequestDtos;

public record UpdateProfileCommand(Guid UserId, string FirstName, string LastName, string Email);
