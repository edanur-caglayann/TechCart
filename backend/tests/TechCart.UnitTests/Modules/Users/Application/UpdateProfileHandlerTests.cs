using Moq;
using TechCart.Users.Application.Profile;
using TechCart.Users.Domain.Entities;
using TechCart.Users.Domain.Exceptions;
using TechCart.Users.Domain.Repositories;
using Xunit;

namespace TechCart.UnitTests.Modules.Users.Application;

public class UpdateProfileHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userWriteRepositoryMock = new Mock<IUserWriteRepository>();
        userWriteRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new UpdateProfileHandler(userWriteRepositoryMock.Object);
        var command = new UpdateProfileCommand(Guid.NewGuid(), "Ada", "Lovelace", "ada@example.com");

        // Act + Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowEmailAlreadyExistsException_WhenNewEmailBelongsToAnotherUser()
    {
        // Arrange: gerçek bir User nesnesi oluşturuyoruz (factory metodu üzerinden)
        var existingUser = User.Register("Ada", "Lovelace", "ada@example.com", "hashed-password");

        var userWriteRepositoryMock = new Mock<IUserWriteRepository>();
        userWriteRepositoryMock
            .Setup(r => r.GetByIdAsync(existingUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // excludeUserId parametresinin GERÇEKTEN kullanıcının kendi id'siyle
        // çağrıldığını da bu setup doğruluyor (It.IsAny değil, existingUser.Id).
        userWriteRepositoryMock
            .Setup(r => r.ExistsByEmailAsync("baskasinin@example.com", It.IsAny<CancellationToken>(), existingUser.Id))
            .ReturnsAsync(true);

        var handler = new UpdateProfileHandler(userWriteRepositoryMock.Object);
        var command = new UpdateProfileCommand(existingUser.Id, "Ada", "Lovelace", "baskasinin@example.com");

        // Act + Assert
        await Assert.ThrowsAsync<EmailAlreadyExistsException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldUpdateProfile_WhenDataIsValid()
    {
        // Arrange
        var existingUser = User.Register("Ada", "Lovelace", "ada@example.com", "hashed-password");

        var userWriteRepositoryMock = new Mock<IUserWriteRepository>();
        userWriteRepositoryMock
            .Setup(r => r.GetByIdAsync(existingUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        userWriteRepositoryMock
            .Setup(r => r.ExistsByEmailAsync("grace@example.com", It.IsAny<CancellationToken>(), existingUser.Id))
            .ReturnsAsync(false);

        var handler = new UpdateProfileHandler(userWriteRepositoryMock.Object);
        var command = new UpdateProfileCommand(existingUser.Id, "Grace", "Hopper", "grace@example.com");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: hem dönen DTO'yu hem entity'nin gerçekten değiştiğini kontrol ediyoruz.
        Assert.Equal("Grace", result.FirstName);
        Assert.Equal("grace@example.com", result.Email);
        Assert.Equal("Grace", existingUser.FirstName);

        userWriteRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}