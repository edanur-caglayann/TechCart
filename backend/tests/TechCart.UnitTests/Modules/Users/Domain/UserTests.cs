using TechCart.Users.Domain.Entities;
using Xunit;

namespace TechCart.UnitTests.Modules.Users.Domain;

public class UserTests
{
    // [Fact]: xUnit'e "bu metot tek bir test senaryosu" demenin yolu.
    
    // Kayit olan kullaniciya Customer rolu atanmasi 
    [Fact]
    public void Register_ShouldAlwaysAssignCustomerRole()
    {
        // Arrange: test için sabit değerler
        var firstName = "Eda";
        var lastName = "Caglayan";
        var email = "eda@example.com";
        var passwordHash = "hashed-password"; 

        // Act: test edilecek olan  "registeer" metodu
        var user = User.Register(firstName, lastName, email, passwordHash);

        // Register'a rol parametre verilmiyor, default olarak customer ataniyor. Bunu dogruluyoruz.
        Assert.Equal(UserRole.Customer, user.Role);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(email, user.Email);
    }

    // Admin kullanicisi olusturulmasi 
    [Fact]
    public void CreateAdmin_ShouldAssignAdminRole()
    {
        var user = User.CreateAdmin("Admin", "User", "admin@example.com", "hashed-password");

        Assert.Equal(UserRole.Admin, user.Role);
    }

    // profil bilgilerinin guncellenmesi
    [Fact]
    public void UpdateProfile_ShouldUpdateFieldsAndSetUpdatedAt()
    {
        // Arrange: önce normal bir kullanıcı oluşturuyoruz
        var user = User.Register("Eda", "Nur", "eda@example.com", "hashed-password");

        // Act
        user.UpdateProfile("Grace", "Hopper", "grace@example.com");

        // Assert: hem alanların değiştiğini hem "ne zaman güncellendi" (MarkUpdated
        // üzerinden AuditableEntity'den gelen UpdatedAt) bilgisinin dolduğunu kontrol ediyoruz.
        Assert.Equal("Grace", user.FirstName);
        Assert.Equal("Hopper", user.LastName);
        Assert.Equal("grace@example.com", user.Email);
        Assert.NotNull(user.UpdatedAt); // ilk oluşturulduğunda null'dı
    }

    [Fact]
    public void ChangePassword_ShouldUpdatePasswordHashAndSetUpdatedAt()
    {
        // Arrange
        var user = User.Register("Eda", "Nur", "ada@example.com", "old-hash");

        // Act
        user.ChangePassword("new-hash");

        // Assert
        Assert.Equal("new-hash", user.PasswordHash);
        Assert.NotNull(user.UpdatedAt);
    }
}