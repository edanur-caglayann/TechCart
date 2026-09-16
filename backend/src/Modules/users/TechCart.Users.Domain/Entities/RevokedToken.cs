using TechCart.SharedKernel.Entities;

namespace TechCart.Users.Domain.Entities;

public class RevokedToken : BaseEntity
{
    public string Jti { get; private set; } = default!;
    public Guid UserId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime RevokedAt { get; private set; }

    private RevokedToken() { }

    private RevokedToken(string jti, Guid userId, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        Jti = jti;
        UserId = userId;
        ExpiresAt = expiresAt;
        RevokedAt = DateTime.UtcNow;
    }

    public static RevokedToken Revoke(string jti, Guid userId, DateTime expiresAt)
        => new(jti, userId, expiresAt);
}
