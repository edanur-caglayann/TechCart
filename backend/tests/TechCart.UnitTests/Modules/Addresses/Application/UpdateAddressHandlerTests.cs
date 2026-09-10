using Moq;
using TechCart.Addresses.Application.Update;
using TechCart.Addresses.Domain.Entities;
using TechCart.Addresses.Domain.Exceptions;
using TechCart.Addresses.Domain.Repositories;
using Xunit;

namespace TechCart.UnitTests.Modules.Addresses.Application;

public class UpdateAddressHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowAddressNotFoundException_WhenAddressDoesNotBelongToUser()
    {
        // Arrange: GetByIdAsync addressId+userId'yi BİRLİKTE arıyordu (IDOR koruması) 
        var addressWriteRepositoryMock = new Mock<IAddressWriteRepository>();
        addressWriteRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Address?)null);

        var handler = new UpdateAddressHandler(addressWriteRepositoryMock.Object);
        var command = new UpdateAddressCommand(Guid.NewGuid(), Guid.NewGuid(), "Ev", "Ada Lovelace",
            "05551234567", "İstanbul", "Kadıköy", "Moda", "Sokak 1", "34710");

        // Act + Assert
        await Assert.ThrowsAsync<AddressNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldUpdateAddress_WhenAddressBelongsToUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingAddress = Address.Create(userId, "Ev", "Ada Lovelace", "05551234567",
            "İstanbul", "Kadıköy", "Moda", "Sokak 1", "34710", isDefault: true);

        var addressWriteRepositoryMock = new Mock<IAddressWriteRepository>();
        addressWriteRepositoryMock
            .Setup(r => r.GetByIdAsync(existingAddress.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAddress);

        var handler = new UpdateAddressHandler(addressWriteRepositoryMock.Object);
        var command = new UpdateAddressCommand(existingAddress.Id, userId, "Yeni Ev", "Grace Hopper",
            "05559876543", "Ankara", "Çankaya", "Kızılay", "Cadde 5", "06420");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Yeni Ev", result.Title);
        Assert.Equal("Kızılay", result.Neighborhood);
        addressWriteRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}