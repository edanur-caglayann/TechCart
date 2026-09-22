using TechCart.Users.Domain.Entities;

namespace TechCart.Users.Application.Dtos.ResponseDtos;

public record UserCredentialsResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PasswordHash,
    UserRole Role);
