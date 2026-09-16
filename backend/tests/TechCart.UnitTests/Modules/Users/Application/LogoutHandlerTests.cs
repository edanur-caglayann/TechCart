using Moq;
using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Auth;
using Xunit;

namespace TechCart.UnitTests.Modules.Users.Application;

public class LogoutHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRevokeCurrentToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTime.UtcNow.AddMinutes(30);
        var revokedTokenRepositoryMock = new Mock<IRevokedTokenRepository>();
        var handler = new LogoutHandler(revokedTokenRepositoryMock.Object);

        // Act
        await handler.Handle(new LogoutCommand("token-jti", userId, expiresAt), CancellationToken.None);

        // Assert
        revokedTokenRepositoryMock.Verify(
            r => r.RevokeAsync("token-jti", userId, expiresAt, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotThrow_WhenSameTokenIsRevokedAgain()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiresAt = DateTime.UtcNow.AddMinutes(30);
        var revokedTokenRepositoryMock = new Mock<IRevokedTokenRepository>();
        var handler = new LogoutHandler(revokedTokenRepositoryMock.Object);
        var command = new LogoutCommand("token-jti", userId, expiresAt);

        // Act
        await handler.Handle(command, CancellationToken.None);
        await handler.Handle(command, CancellationToken.None);

        // Assert
        revokedTokenRepositoryMock.Verify(
            r => r.RevokeAsync("token-jti", userId, expiresAt, It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }
}
