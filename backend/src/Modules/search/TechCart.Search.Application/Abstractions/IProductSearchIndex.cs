using TechCart.Search.Application.Documents;

namespace TechCart.Search.Application.Abstractions;

public record ProductSearchPage(IReadOnlyList<ProductSearchDocument> Items, long TotalCount);
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
    
    
    // searchText alanında arar. Harf içeren kelimeler yazım hatasına toleranslı,
    // sayı olan kelimeler tam eşleşme ister 
    Task<ProductSearchPage> SearchAsync(string term, int page, int pageSize, CancellationToken ct);
    
    // searchText'in ön ek alt alanlarında arar. "blu" yazınca "Bluetooth"lu
    // ürünleri bulur — kullanıcı henüz yazarken, Enter'a basmadan çalışır.
    Task<IReadOnlyList<string>> SuggestAsync(string term, int limit, CancellationToken ct);

}