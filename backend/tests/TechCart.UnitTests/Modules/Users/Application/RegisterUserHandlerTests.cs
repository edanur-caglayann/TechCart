using Moq;
using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Register;
using TechCart.Users.Domain.Entities;
using TechCart.Users.Domain.Exceptions;
using TechCart.Users.Domain.Repositories;
using Xunit;

namespace TechCart.UnitTests.Modules.Users.Application;

// RegisterUserHandler, IUserWriteRepository/IPasswordHasher/ITokenGenerator
// ARAYÜZLERİYLE konuşuyor, gerçek implementasyonlarıyla değil — bu yüzden test
// sırasında bunların yerine Moq'un ürettiği "sahte" versiyonları veriyoruz.
// Böylece gerçek Postgres'e hiç dokunmadan "Handler doğru karar veriyor mu"
// diye test edebiliyoruz.
public class RegisterUserHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowEmailAlreadyExistsException_WhenEmailIsTaken()
    {
        // Arrange: sahte repository'ye "bu e-posta zaten var" cevabını VERMESİNİ söylüyoruz.
        var userWriteRepositoryMock = new Mock<IUserWriteRepository>();
        userWriteRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Guid?>()))
            .ReturnsAsync(true);

        var passwordHasherMock = new Mock<IPasswordHasher>();
        var tokenGeneratorMock = new Mock<ITokenGenerator>();

        var handler = new RegisterUserHandler(userWriteRepositoryMock.Object, passwordHasherMock.Object, tokenGeneratorMock.Object);
        var command = new RegisterUserCommand("Eda", "Nur", "eda@example.com", "Password123!");

        // Act + Assert: Assert.ThrowsAsync, hem kodu çalıştırıp hem "bu exception
        // fırlamalı" diye beklemenin yolu.
        await Assert.ThrowsAsync<EmailAlreadyExistsException>(() => handler.Handle(command, CancellationToken.None));

        // Ekstra doğrulama: e-posta zaten kayıtlıysa AddAsync'in HİÇ çağrılmadığından
        // emin oluyoruz — yanlışlıkla ikinci bir kayıt oluşmadığını garanti eder.
        userWriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateUserAndReturnToken_WhenEmailIsAvailable()
    {
        // Arrange
        var userWriteRepositoryMock = new Mock<IUserWriteRepository>();
        userWriteRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Guid?>()))
            .ReturnsAsync(false);

        var passwordHasherMock = new Mock<IPasswordHasher>();
        passwordHasherMock
            .Setup(h => h.Hash("Password123!"))
            .Returns("hashed-password"); // gerçek BCrypt çalışmasın diye sahte hash döndürüyoruz

        var tokenGeneratorMock = new Mock<ITokenGenerator>();
        tokenGeneratorMock
            .Setup(t => t.GenerateToken(It.IsAny<Guid>(), "Ada", "Lovelace", "ada@example.com", UserRole.Customer))
            .Returns("fake-jwt-token");

        var handler = new RegisterUserHandler(userWriteRepositoryMock.Object, passwordHasherMock.Object, tokenGeneratorMock.Object);
        var command = new RegisterUserCommand("Ada", "Lovelace", "ada@example.com", "Password123!");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: dönen sonuç doğru mu
        Assert.Equal("fake-jwt-token", result.Token);
        Assert.Equal("Ada", result.User.FirstName);
        Assert.Equal("Customer", result.User.Role);

        // Kullanıcının gerçekten kaydedilmeye çalışıldığını doğruluyoruz.
        userWriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        userWriteRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
