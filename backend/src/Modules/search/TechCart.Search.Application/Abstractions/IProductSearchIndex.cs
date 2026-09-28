using TechCart.Search.Application.Documents;

namespace TechCart.Search.Application.Abstractions;

public interface IProductSearchIndex
{
    // İndeksi (varsa) siler ve şemasıyla birlikte yeniden oluşturur.
    // true = indeks başarıyla oluştu.
    Task<bool> RecreateAsync(CancellationToken ct);

    // Verilen dokümanları tek bir toplu istekle indekse yazar. Her doküman kendi
    // Id'siyle yazılır: aynı ürün tekrar yazılırsa kopyası oluşmaz, üzerine yazılır.
    // Herhangi bir doküman reddedilirse hata fırlatır.
    Task IndexManyAsync(IReadOnlyCollection<ProductSearchDocument> documents, CancellationToken ct);
}