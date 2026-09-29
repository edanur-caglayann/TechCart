using TechCart.Search.Application.Documents;

namespace TechCart.Search.Application.Abstractions;

public interface IProductSearchIndex
{
    // Elasticsearch’te 101 id'li urun var. 102 id'li urun PostgreSQL’den silinmiş ama Elasticsearch’te hâlâ duruyor
    // Bu metot ile Elasticsearch’teki ürün indeksi boş hâle gelir.
    // kod PostgreSQL’de şu anda bulunan ürünleri yeniden okuyup yazar:
    // 101 geri gelir; 102 ise PostgreSQL’den okunamadığı için geri gelmez.
    Task<bool> RecreateAsync(CancellationToken ct);

    // Verilen dokümanları tek bir toplu istekle indekse yazar. Her doküman kendi
    // Id'siyle yazılır: aynı ürün tekrar yazılırsa kopyası oluşmaz, üzerine yazılır.
    // Herhangi bir doküman reddedilirse hata fırlatır.
    Task IndexManyAsync(IReadOnlyCollection<ProductSearchDocument> documents, CancellationToken ct);
}