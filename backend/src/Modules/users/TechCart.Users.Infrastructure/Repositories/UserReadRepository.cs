using Microsoft.EntityFrameworkCore;
using TechCart.Users.Application.Abstractions;

namespace TechCart.Users.Infrastructure.Repositories;

public class UserReadRepository : IUserReadRepository
{
    private readonly UsersDbContext _dbContext;
    public UserReadRepository(UsersDbContext dbContext) => _dbContext = dbContext;

    public Task<UserCredentialsDto?> GetCredentialsByEmailAsync(string email, CancellationToken ct)
        => _dbContext.Users
            .AsNoTracking()
            .Where(u => EF.Functions.ILike(u.Email, email))
            .Select(u => new UserCredentialsDto(u.Id, u.FirstName, u.LastName, u.Email, u.PasswordHash, u.Role))
            .FirstOrDefaultAsync(ct);

    public Task<UserProfileDto?> GetProfileByIdAsync(Guid id, CancellationToken ct)
        => _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserProfileDto(u.Id, u.FirstName, u.LastName, u.Email, u.Role.ToString(), u.CreatedAt))
            .FirstOrDefaultAsync(ct);
}