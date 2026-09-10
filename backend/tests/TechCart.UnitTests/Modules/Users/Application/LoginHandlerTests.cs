using Moq;
using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Login;
using TechCart.Users.Domain.Entities;
using TechCart.Users.Domain.Exceptions;
using Xunit;

namespace TechCart.UnitTests.Modules.Users.Application;

public class LoginHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowInvalidCredentialsException_WhenUserNotFound()
    {
        // Arrange: sahte repository "böyle bir kullanıcı yok" (null) döndürsün.
        var userReadRepositoryMock = new Mock<IUserReadRepository>();
        userReadRepositoryMock
            .Setup(r => r.GetCredentialsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserCredentialsDto?)null);

        var handler = new LoginHandler(userReadRepositoryMock.Object, new Mock<IPasswordHasher>().Object, new Mock<ITokenGenerator>().Object);
        var command = new LoginCommand("olmayan@example.com", "AnyPassword1!");

        // Act + Assert
        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidCredentialsException_WhenPasswordIsWrong()
    {
        // Arrange: kullanıcı bulunuyor ama şifre doğrulaması BAŞARISIZ olacak.
        var credentials = new UserCredentialsDto(Guid.NewGuid(), "Ada", "Lovelace", "ada@example.com", "stored-hash", UserRole.Customer);

        var userReadRepositoryMock = new Mock<IUserReadRepository>();
        userReadRepositoryMock
            .Setup(r => r.GetCredentialsByEmailAsync("ada@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(credentials);

        var passwordHasherMock = new Mock<IPasswordHasher>();
        passwordHasherMock
            .Setup(h => h.Verify("wrong-password", "stored-hash"))
            .Returns(false);

        var handler = new LoginHandler(userReadRepositoryMock.Object, passwordHasherMock.Object, new Mock<ITokenGenerator>().Object);
        var command = new LoginCommand("ada@example.com", "wrong-password");

        // Act + Assert: hem "kullanıcı yok" hem "şifre yanlış" AYNI exception'ı
        // fırlatmalı — login'de bilerek kurduğumuz enumeration korumasını burada da doğruluyoruz.
        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResult_WhenCredentialsAreValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var credentials = new UserCredentialsDto(userId, "Ada", "Lovelace", "ada@example.com", "stored-hash", UserRole.Customer);

        var userReadRepositoryMock = new Mock<IUserReadRepository>();
        userReadRepositoryMock
            .Setup(r => r.GetCredentialsByEmailAsync("ada@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(credentials);

        var passwordHasherMock = new Mock<IPasswordHasher>();
        passwordHasherMock
            .Setup(h => h.Verify("correct-password", "stored-hash"))
            .Returns(true);

        var tokenGeneratorMock = new Mock<ITokenGenerator>();
        tokenGeneratorMock
            .Setup(t => t.GenerateToken(userId, "Ada", "Lovelace", "ada@example.com", UserRole.Customer))
            .Returns("fake-jwt-token");

        var handler = new LoginHandler(userReadRepositoryMock.Object, passwordHasherMock.Object, tokenGeneratorMock.Object);
        var command = new LoginCommand("ada@example.com", "correct-password");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("fake-jwt-token", result.Token);
        Assert.Equal(userId, result.User.Id);
    }
}