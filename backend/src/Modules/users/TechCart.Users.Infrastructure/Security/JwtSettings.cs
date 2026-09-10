namespace TechCart.Users.Infrastructure.Security;

public class JwtSettings
{
    public string Secret { get; set; } = default!; // token'i imzalamak ve dogrulamak icin kullanilan gizli anahtar
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!; // token hangi uygulama icin uretildi
    public int ExpiryMinutes { get; set; } // token kac dk gecerli
}