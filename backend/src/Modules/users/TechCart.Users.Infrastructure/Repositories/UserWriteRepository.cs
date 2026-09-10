using Microsoft.EntityFrameworkCore;
using TechCart.Users.Domain.Entities;
using TechCart.Users.Domain.Repositories;

namespace TechCart.Users.Infrastructure.Repositories;

// IUserRepository'nin EF Core ile çalışan gerçek implementasyonu
public class UserWriteRepository : IUserWriteRepository
{
    private readonly UsersDbContext _dbContext;
    public UserWriteRepository(UsersDbContext dbContext) => _dbContext = dbContext;

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct, Guid? excludeUserId = null)
        => _dbContext.Users.AnyAsync(u =>
            EF.Functions.ILike(u.Email, email) && (excludeUserId == null || u.Id != excludeUserId), ct);

    public async Task AddAsync(User user, CancellationToken ct)
        => await _dbContext.Users.AddAsync(user, ct);
    
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        => _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
}