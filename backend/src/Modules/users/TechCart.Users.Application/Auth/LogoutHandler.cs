using TechCart.Users.Application.Abstractions;

namespace TechCart.Users.Application.Auth;

public class LogoutHandler
{
    private readonly IRevokedTokenRepository _revokedTokenRepository;

    public LogoutHandler(IRevokedTokenRepository revokedTokenRepository)
        => _revokedTokenRepository = revokedTokenRepository;

    public Task Handle(LogoutCommand command, CancellationToken ct)
        => _revokedTokenRepository.RevokeAsync(command.Jti, command.UserId, command.ExpiresAt, ct);
}

public record LogoutCommand(string Jti, Guid UserId, DateTime ExpiresAt);
