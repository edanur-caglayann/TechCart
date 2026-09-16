namespace TechCart.Users.Application.Abstractions;

public interface IRevokedTokenRepository
{
    Task RevokeAsync(string jti, Guid userId, DateTime expiresAt, CancellationToken ct);
    Task<bool> IsRevokedAsync(string jti, CancellationToken ct);
}
