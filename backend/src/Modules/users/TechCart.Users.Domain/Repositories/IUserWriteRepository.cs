using TechCart.Users.Domain.Entities;

namespace TechCart.Users.Domain.Repositories;

// User için hangi veritabanı işlemlerine ihtiyacımız olduğunu tanımlayan arayüz.
// sadece bu arayüze bağımlı olacak, gerçek implementasyonu (EF Core ile) Infrastructure verecek.
public interface IUserWriteRepository
{
    // Login ve e-posta zaten kayıtlı mı kontrolü için
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct, Guid? excludeUserId = null);

    // Yeni kullanıcıyı EF Core'un change tracker'ına ekler — henüz DB'ye yazmaz
    Task AddAsync(User user, CancellationToken ct);
    
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);

    // Değişiklikleri veritabanına yazar 
    Task SaveChangesAsync(CancellationToken ct);
}