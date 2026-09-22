using TechCart.Payments.Domain.Entities;

namespace TechCart.Payments.Domain.Repositories;

public interface IPaymentWriteRepository
{
    // kullanıcı, ödemesi zaten başarıyla işlenmiş olan callback sayfasını yenilerse,
    // ya da ağ bir isteği tekrar gönderirse endpoint'e ayni istek yine gider. 
    // Bu islem zaten yapildi demesi icin ara bir kontrol
    Task<bool> ExistsByProviderReferenceAsync(string providerReference, CancellationToken ct);

    Task AddAsync(Payment payment, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
