using Moq;
using TechCart.Addresses.Application.Abstractions;
using TechCart.Addresses.Application.List;
using Xunit;

namespace TechCart.UnitTests.Modules.Addresses.Application;

public class ListMyAddressesHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAddressesFromRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var addresses = new List<AddressDto>
        {
            new(Guid.NewGuid(), "Ev", "Ada Lovelace", "05551234567", "İstanbul", "Kadıköy", "Moda", "Sokak 1", "34710", true)
        };

        var addressReadRepositoryMock = new Mock<IAddressReadRepository>();
        addressReadRepositoryMock
            .Setup(r => r.GetMyAddressesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(addresses);

        var handler = new ListMyAddressesHandler(addressReadRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new ListMyAddressesQuery(userId), CancellationToken.None);

        // Assert: Handler burada ekstra bir mantık uygulamıyor, sadece repository'i
        // çağırıp sonucu döndürüyor — bunu doğruluyoruz.
        Assert.Single(result);
        Assert.Equal("Ev", result[0].Title);
    }
}