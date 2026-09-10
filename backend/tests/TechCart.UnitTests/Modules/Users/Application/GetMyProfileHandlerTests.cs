using Moq;
using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Profile;
using TechCart.Users.Domain.Exceptions;
using Xunit;

namespace TechCart.UnitTests.Modules.Users.Application;

public class GetMyProfileHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowUserNotFoundException_WhenProfileDoesNotExist()
    {
        // Arrange: sahte repository "bu id'de kullanıcı yok" (null) döndürsün.
        var userReadRepositoryMock = new Mock<IUserReadRepository>();
        userReadRepositoryMock
            .Setup(r => r.GetProfileByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfileDto?)null);

        var handler = new GetMyProfileHandler(userReadRepositoryMock.Object);
        var query = new GetMyProfileQuery(Guid.NewGuid());

        // Act + Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnProfile_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profile = new UserProfileDto(userId, "Ada", "Lovelace", "ada@example.com", "Customer", DateTime.UtcNow);

        var userReadRepositoryMock = new Mock<IUserReadRepository>();
        userReadRepositoryMock
            .Setup(r => r.GetProfileByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var handler = new GetMyProfileHandler(userReadRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetMyProfileQuery(userId), CancellationToken.None);

        // Assert
        Assert.Equal("Ada", result.FirstName);
        Assert.Equal(userId, result.Id);
    }
}