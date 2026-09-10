namespace TechCart.Users.Application.Abstractions;

// Şifre hash'leme için soyutlama. Application katmanı "nasıl hash'leneceğini" bilmiyor,
// sadece bu arayüzü kullanıyor — gerçek implementasyon (BCrypt ile) Infrastructure'da.
public interface IPasswordHasher
{
    string Hash(string plainPassword);
    bool Verify(string plainPassword, string hash);
}