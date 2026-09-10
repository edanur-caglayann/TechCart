using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TechCart.Users.Application.Abstractions;
using TechCart.Users.Domain.Entities;

namespace TechCart.Users.Infrastructure.Security;

// kullanıcı bilgilerini alır, JWT oluşturur,
// gizli anahtarla imzalar ve string olarak döndürür.
public class JwtTokenGenerator : ITokenGenerator
{
    private readonly JwtSettings _settings;
    public JwtTokenGenerator(IOptions<JwtSettings> options) => _settings = options.Value;

    // kullanici bilgilerini alarak JWT uretir
    public string GenerateToken(Guid userId, string firstName, string lastName, string email, UserRole role)
    {
        // claims, token'in icinde tasinacak kullanici bilgileridir.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), // token'in sahibi
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.GivenName, firstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, lastName),
            new Claim(ClaimTypes.Role, role.ToString()),
            // jti-> JWT Id -> ayni kullanici tekrar giris yapsa bile her seferinde farkli bir
            // token id'si uretilir
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // appsettings.json içinden gelen Secret metni byte donusturulerek imza olusturulur
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        
        // Token'in hangi anahtar ve alg. ile imzalanacagi
        // HmacSha256 simetrik bir algoritmadır: Token oluşturulurken ve doğrulanırken aynı Secret kullanılır.
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // kullaniciya verilecek jwt nesnesi
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims, // token icindeki kullanici bilgileri
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes), // token'in ne zaman gecersiz olacagi
            signingCredentials: creds); // token'in secret anahtariyla imzalanir

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}