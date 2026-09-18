using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Dtos.RequestDtos;

namespace TechCart.Users.Application.Auth;

public class LogoutHandler
{
    private readonly IRevokedTokenRepository _revokedTokenRepository;

    public LogoutHandler(IRevokedTokenRepository revokedTokenRepository)
        => _revokedTokenRepository = revokedTokenRepository;

    public Task Handle(LogoutCommand command, CancellationToken ct)
        => _revokedTokenRepository.RevokeAsync(command.Jti, command.UserId, command.ExpiresAt, ct);
}
