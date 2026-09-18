using Microsoft.EntityFrameworkCore;
using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Dtos.ResponseDtos;

namespace TechCart.Users.Infrastructure.Repositories;

public class UserReadRepository : IUserReadRepository
{
    private readonly UsersDbContext _dbContext;
    public UserReadRepository(UsersDbContext dbContext) => _dbContext = dbContext;

    public Task<UserCredentialsResponse?> GetCredentialsByEmailAsync(string email, CancellationToken ct)
        => _dbContext.Users
            .AsNoTracking()
            .Where(u => EF.Functions.ILike(u.Email, email))
            .Select(u => new UserCredentialsResponse(u.Id, u.FirstName, u.LastName, u.Email, u.PasswordHash, u.Role))
            .FirstOrDefaultAsync(ct);

    public Task<UserProfileResponse?> GetProfileByIdAsync(Guid id, CancellationToken ct)
        => _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserProfileResponse(u.Id, u.FirstName, u.LastName, u.Email, u.Role.ToString(), u.CreatedAt))
            .FirstOrDefaultAsync(ct);
}
