using TechCart.Addresses.Domain.Entities;
using Xunit;

namespace TechCart.UnitTests.Modules.Addresses.Domain;

public class AddressTests
{
    [Fact]
    public void Create_ShouldSetIsDefaultAsGiven_WhenTrue()
    {
        // Act: Create'in isDefault parametresini olduğu gibi kabul ettiğini doğruluyoruz 
        var address = Address.Create(Guid.NewGuid(), "Ev", "Ada Lovelace", "05551234567",
            "İstanbul", "Kadıköy", "Moda", "Sokak 1", "34710", isDefault: true);

        // Assert
        Assert.True(address.IsDefault);
    }

    [Fact]
    public void Create_ShouldSetIsDefaultAsGiven_WhenFalse()
    {
        var address = Address.Create(Guid.NewGuid(), "İş", "Ada Lovelace", "05551234567",
            "İstanbul", "Şişli", "Mecidiyeköy", "Cadde 2", "34394", isDefault: false);

        Assert.False(address.IsDefault);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateFieldsAndSetUpdatedAt()
    {
        // Arrange
        var address = Address.Create(Guid.NewGuid(), "Ev", "Ada Lovelace", "05551234567",
            "İstanbul", "Kadıköy", "Moda", "Sokak 1", "34710", isDefault: true);

        // Act
        address.UpdateDetails("Yeni Ev", "Grace Hopper", "05559876543",
            "Ankara", "Çankaya", "Kızılay", "Yeni Cadde No:5", "06420");

        // Assert
        Assert.Equal("Yeni Ev", address.Title);
        Assert.Equal("Kızılay", address.Neighborhood);
        Assert.NotEqual(default, address.UpdatedAt); 
    }

    [Fact]
    public void MarkAsDefault_ShouldSetIsDefaultToTrue()
    {
        var address = Address.Create(Guid.NewGuid(), "Ev", "Ada Lovelace", "05551234567",
            "İstanbul", "Kadıköy", "Moda", "Sokak 1", "34710", isDefault: false);

        address.MarkAsDefault();

        Assert.True(address.IsDefault);
    }

    [Fact]
    public void UnmarkAsDefault_ShouldSetIsDefaultToFalse()
    {
        var address = Address.Create(Guid.NewGuid(), "Ev", "Ada Lovelace", "05551234567",
            "İstanbul", "Kadıköy", "Moda", "Sokak 1", "34710", isDefault: true);

        address.UnmarkAsDefault();

        Assert.False(address.IsDefault);
    }
}