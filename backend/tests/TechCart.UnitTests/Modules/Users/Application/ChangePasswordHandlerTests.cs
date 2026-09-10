using Moq;
using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Profile;
using TechCart.Users.Domain.Entities;
using TechCart.Users.Domain.Exceptions;
using TechCart.Users.Domain.Repositories;
using Xunit;

namespace TechCart.UnitTests.Modules.Users.Application;

public class ChangePasswordHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userWriteRepositoryMock = new Mock<IUserWriteRepository>();
        userWriteRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new ChangePasswordHandler(userWriteRepositoryMock.Object, new Mock<IPasswordHasher>().Object);
        var command = new ChangePasswordCommand(Guid.NewGuid(), "current", "new-password");

        // Act + Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowIncorrectCurrentPasswordException_WhenCurrentPasswordIsWrong()
    {
        // Arrange
        var existingUser = User.Register("Ada", "Lovelace", "ada@example.com", "stored-hash");

        var userWriteRepositoryMock = new Mock<IUserWriteRepository>();
        userWriteRepositoryMock
            .Setup(r => r.GetByIdAsync(existingUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        var passwordHasherMock = new Mock<IPasswordHasher>();
        passwordHasherMock
            .Setup(h => h.Verify("wrong-current-password", "stored-hash"))
            .Returns(false);

        var handler = new ChangePasswordHandler(userWriteRepositoryMock.Object, passwordHasherMock.Object);
        var command = new ChangePasswordCommand(existingUser.Id, "wrong-current-password", "new-password");

        // Act + Assert
        await Assert.ThrowsAsync<IncorrectCurrentPasswordException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldUpdatePasswordHash_WhenCurrentPasswordIsCorrect()
    {
        // Arrange
        var existingUser = User.Register("Ada", "Lovelace", "ada@example.com", "old-hash");

        var userWriteRepositoryMock = new Mock<IUserWriteRepository>();
        userWriteRepositoryMock
            .Setup(r => r.GetByIdAsync(existingUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        var passwordHasherMock = new Mock<IPasswordHasher>();
        passwordHasherMock.Setup(h => h.Verify("correct-current-password", "old-hash")).Returns(true);
        passwordHasherMock.Setup(h => h.Hash("new-password")).Returns("new-hash");

        var handler = new ChangePasswordHandler(userWriteRepositoryMock.Object, passwordHasherMock.Object);
        var command = new ChangePasswordCommand(existingUser.Id, "correct-current-password", "new-password");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert: entity'nin PasswordHash'i gerçekten değişti mi, kaydetme tam olarak 1 kez çağrıldı mı.
        Assert.Equal("new-hash", existingUser.PasswordHash);
        userWriteRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}