using TechCart.Users.Application.Abstractions;

namespace TechCart.Users.Infrastructure.Security;

// IPasswordHasher'ın BCrypt tabanlı gerçek implementasyonu.
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string plainPassword)
        => BCrypt.Net.BCrypt.HashPassword(plainPassword);

    // Kullanıcının girdiği düz şifreyi, veritabanındaki hash ile karşılaştırır.
    public bool Verify(string plainPassword, string hash)
        => BCrypt.Net.BCrypt.Verify(plainPassword, hash);
}
