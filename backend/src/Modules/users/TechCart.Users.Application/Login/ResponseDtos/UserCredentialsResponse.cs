using TechCart.Users.Domain.Entities;

namespace TechCart.Users.Application.Login.ResponseDtos;

public record UserCredentialsResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PasswordHash,
    UserRole Role);
