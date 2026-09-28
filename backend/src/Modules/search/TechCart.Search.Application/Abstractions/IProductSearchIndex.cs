namespace TechCart.Search.Application.Abstractions;

public interface IProductSearchIndex
{
    // İndeksi (varsa) siler ve şemasıyla birlikte yeniden oluşturur.
    // true = indeks başarıyla oluştu.
    Task<bool> RecreateAsync(CancellationToken ct);
}