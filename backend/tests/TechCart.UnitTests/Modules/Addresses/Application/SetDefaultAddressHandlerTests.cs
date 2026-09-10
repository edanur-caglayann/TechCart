using Moq;
using TechCart.Addresses.Application.SetDefault;
using TechCart.Addresses.Domain.Entities;
using TechCart.Addresses.Domain.Exceptions;
using TechCart.Addresses.Domain.Repositories;
using Xunit;

namespace TechCart.UnitTests.Modules.Addresses.Application;

public class SetDefaultAddressHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowAddressNotFoundException_WhenAddressIsNotInUsersList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var addressWriteRepositoryMock = new Mock<IAddressWriteRepository>();
        addressWriteRepositoryMock
            .Setup(r => r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Address>());

        var handler = new SetDefaultAddressHandler(addressWriteRepositoryMock.Object);
        var command = new SetDefaultAddressCommand(Guid.NewGuid(), userId);

        // Act + Assert
        await Assert.ThrowsAsync<AddressNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldMarkOnlyTargetAsDefault_AndUnmarkAllOthers()
    {
        // Arrange: üç adres, ilki şu an varsayılan. İkinciyi varsayılan yapacağız.
        var userId = Guid.NewGuid();

        var firstAddress = Address.Create(userId, "Ev", "Ada Lovelace", "05551234567",
            "İstanbul", "Kadıköy", "Moda", "Sokak 1", "34710", isDefault: true);
        var secondAddress = Address.Create(userId, "İş", "Ada Lovelace", "05559876543",
            "İstanbul", "Şişli", "Mecidiyeköy", "Cadde 2", "34394", isDefault: false);
        var thirdAddress = Address.Create(userId, "Yazlık", "Ada Lovelace", "05551112233",
            "Muğla", "Bodrum", "Gümbet", "Sokak 3", "48400", isDefault: false);

        var addressWriteRepositoryMock = new Mock<IAddressWriteRepository>();
        addressWriteRepositoryMock
            .Setup(r => r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Address> { firstAddress, secondAddress, thirdAddress });

        var handler = new SetDefaultAddressHandler(addressWriteRepositoryMock.Object);
        var command = new SetDefaultAddressCommand(secondAddress.Id, userId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert: "aynı anda yalnızca bir varsayılan" kuralının atomik uygulandığını doğruluyoruz.
        Assert.False(firstAddress.IsDefault);
        Assert.True(secondAddress.IsDefault);
        Assert.False(thirdAddress.IsDefault);

        // Dönen listede de tam olarak bir tane IsDefault:true olmalı.
        Assert.Equal(3, result.Count);
        Assert.Single(result, a => a.IsDefault);
    }
}