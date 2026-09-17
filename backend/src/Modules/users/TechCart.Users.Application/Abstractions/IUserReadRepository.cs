using TechCart.Users.Application.Login.ResponseDtos;
using TechCart.Users.Application.Profile.ResponseDtos;

namespace TechCart.Users.Application.Abstractions;

public interface IUserReadRepository
{
    // Login'in ihtiyacı olan minimum alanları döndürür 
    Task<UserCredentialsResponse?> GetCredentialsByEmailAsync(string email, CancellationToken ct);
    
    // profil ekraninin ihtiyaci olan alanlar.
    Task<UserProfileResponse?> GetProfileByIdAsync(Guid id, CancellationToken ct);
}
