using TechCart.Users.Domain.Entities;

namespace TechCart.Users.Application.Abstractions;

public interface IUserReadRepository
{
    // Login'in ihtiyacı olan minimum alanları döndürür 
    Task<UserCredentialsDto?> GetCredentialsByEmailAsync(string email, CancellationToken ct);
    
    // profil ekraninin ihtiyaci olan alanlar.
    Task<UserProfileDto?> GetProfileByIdAsync(Guid id, CancellationToken ct);

}

public record UserCredentialsDto(
    Guid Id, 
    string FirstName, 
    string LastName, 
    string Email, 
    string PasswordHash, 
    UserRole Role
    );
public record UserProfileDto(
    Guid Id, 
    string FirstName, 
    string LastName, 
    string Email, 
    string Role, 
    DateTime CreatedAt);
    