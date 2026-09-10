using Moq;
using TechCart.Addresses.Application.Create;
using TechCart.Addresses.Domain.Entities;
using TechCart.Addresses.Domain.Repositories;
using Xunit;

namespace TechCart.UnitTests.Modules.Addresses.Application;

public class CreateAddressHandlerTests
{
    [Fact]
    public async Task Handle_ShouldSetIsDefaultTrue_WhenItIsUsersFirstAddress()
    {
        // Arrange: kullanıcının hiç adresi yok (CountByUserIdAsync -> 0)
        var userId = Guid.NewGuid();
        var addressWriteRepositoryMock = new Mock<IAddressWriteRepository>();
        addressWriteRepositoryMock
            .Setup(r => r.CountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var handler = new CreateAddressHandler(addressWriteRepositoryMock.Object);
        var command = new CreateAddressCommand(userId, "Ev", "Ada Lovelace", "05551234567",
            "İstanbul", "Kadıköy", "Moda", "Sokak 1", "34710");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: doküman gereği ilk adres otomatik varsayılan olmalı.
        Assert.True(result.IsDefault);
        addressWriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetIsDefaultFalse_WhenUserAlreadyHasAddresses()
    {
        // Arrange: kullanıcının zaten 2 adresi var
        var userId = Guid.NewGuid();
        var addressWriteRepositoryMock = new Mock<IAddressWriteRepository>();
        addressWriteRepositoryMock
            .Setup(r => r.CountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var handler = new CreateAddressHandler(addressWriteRepositoryMock.Object);
        var command = new CreateAddressCommand(userId, "İş", "Ada Lovelace", "05551234567",
            "İstanbul", "Şişli", "Mecidiyeköy", "Cadde 2", "34394");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: ilk adres değil, otomatik varsayılan OLMAMALI.
        Assert.False(result.IsDefault);
    }
}