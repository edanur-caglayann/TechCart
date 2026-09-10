using TechCart.Users.Domain.Entities;

namespace TechCart.Users.Application.Abstractions;

public interface ITokenGenerator
{
    // Kullanıcının bilgilerini alarak bir token üretir
    // ve üretilen token'ı string olarak döndürür.
    string GenerateToken(
        Guid userId, // hangi kullaniciya ait
        string firstName, 
        string lastName, 
        string email, 
        UserRole role);
}