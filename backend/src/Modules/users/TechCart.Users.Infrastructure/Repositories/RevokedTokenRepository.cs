using Microsoft.EntityFrameworkCore;
using Npgsql;
using TechCart.Users.Application.Abstractions;
using TechCart.Users.Domain.Entities;

namespace TechCart.Users.Infrastructure.Repositories;

public class RevokedTokenRepository : IRevokedTokenRepository
{
    private readonly UsersDbContext _dbContext;

    public RevokedTokenRepository(UsersDbContext dbContext)
        => _dbContext = dbContext;

    public async Task RevokeAsync(string jti, Guid userId, DateTime expiresAt, CancellationToken ct)
    {
        var alreadyRevoked = await _dbContext.RevokedTokens.AnyAsync(t => t.Jti == jti, ct);
        if (alreadyRevoked)
        {
            return;
        }

        await _dbContext.RevokedTokens.AddAsync(RevokedToken.Revoke(jti, userId, expiresAt), ct);
        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            _dbContext.ChangeTracker.Clear();
        }
    }

    public Task<bool> IsRevokedAsync(string jti, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        return _dbContext.RevokedTokens
            .AsNoTracking()
            .AnyAsync(t => t.Jti == jti && t.ExpiresAt > now, ct);
    }
}
