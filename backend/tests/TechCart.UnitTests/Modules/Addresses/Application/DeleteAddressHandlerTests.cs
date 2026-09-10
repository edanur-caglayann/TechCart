using Moq;
using TechCart.Addresses.Application.Delete;
using TechCart.Addresses.Domain.Entities;
using TechCart.Addresses.Domain.Exceptions;
using TechCart.Addresses.Domain.Repositories;
using Xunit;

namespace TechCart.UnitTests.Modules.Addresses.Application;

public class DeleteAddressHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowAddressNotFoundException_WhenAddressIsNotInUsersList()
    {
        // Arrange: kullanıcının hiç adresi yok
        var userId = Guid.NewGuid();
        var addressWriteRepositoryMock = new Mock<IAddressWriteRepository>();
        addressWriteRepositoryMock
            .Setup(r => r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Address>());

        var handler = new DeleteAddressHandler(addressWriteRepositoryMock.Object);
        var command = new DeleteAddressCommand(Guid.NewGuid(), userId);

        // Act + Assert
        await Assert.ThrowsAsync<AddressNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldPromoteOldestRemainingAddressToDefault_WhenDeletedAddressWasDefault()
    {
        // Arrange: iki adres var, eski olan varsayılan DEĞİL, yeni olan varsayılan.
        // Silinen adres varsayılansa "en eski kalan" yeni varsayılan olmalı kuralını test ediyoruz.
        var userId = Guid.NewGuid();

        var olderAddress = Address.Create(userId, "Ev", "Ada Lovelace", "05551234567",
            "İstanbul", "Kadıköy", "Moda", "Sokak 1", "34710", isDefault: false);

        // CreatedAt entity içinde otomatik DateTime.UtcNow ile atanıyor. İki Create
        // çağrısı çok hızlı art arda gelirse sistem saatinin çözünürlüğü yüzünden
        // AYNI anı yakalayabilir — Handler'ın "en eski"yi doğru seçtiğini güvenilir
        // test edebilmek için aralarına küçük bir bekleme koyuyoruz.
        Thread.Sleep(10);

        var newerAddress = Address.Create(userId, "İş", "Ada Lovelace", "05559876543",
            "İstanbul", "Şişli", "Mecidiyeköy", "Cadde 2", "34394", isDefault: true);

        var addressWriteRepositoryMock = new Mock<IAddressWriteRepository>();
        addressWriteRepositoryMock
            .Setup(r => r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Address> { olderAddress, newerAddress });

        var handler = new DeleteAddressHandler(addressWriteRepositoryMock.Object);
        // Silinen: newerAddress (şu an varsayılan olan)
        var command = new DeleteAddressCommand(newerAddress.Id, userId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert: kalan tek adres (olderAddress) yeni varsayılan olmalı.
        Assert.True(olderAddress.IsDefault);
        addressWriteRepositoryMock.Verify(r => r.Remove(newerAddress), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotChangeOtherAddressDefaultStatus_WhenDeletedAddressWasNotDefault()
    {
        // Arrange: silinen adres varsayılan değilse, kalanların durumuna DOKUNULMAMALI.
        var userId = Guid.NewGuid();

        var defaultAddress = Address.Create(userId, "Ev", "Ada Lovelace", "05551234567",
            "İstanbul", "Kadıköy", "Moda", "Sokak 1", "34710", isDefault: true);
        var nonDefaultAddress = Address.Create(userId, "İş", "Ada Lovelace", "05559876543",
            "İstanbul", "Şişli", "Mecidiyeköy", "Cadde 2", "34394", isDefault: false);

        var addressWriteRepositoryMock = new Mock<IAddressWriteRepository>();
        addressWriteRepositoryMock
            .Setup(r => r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Address> { defaultAddress, nonDefaultAddress });

        var handler = new DeleteAddressHandler(addressWriteRepositoryMock.Object);
        var command = new DeleteAddressCommand(nonDefaultAddress.Id, userId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert: varsayılan olan adres hiç değişmedi.
        Assert.True(defaultAddress.IsDefault);
    }
}