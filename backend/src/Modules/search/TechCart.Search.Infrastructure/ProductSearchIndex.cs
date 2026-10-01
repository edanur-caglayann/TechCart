using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using TechCart.Search.Application.Abstractions;
using TechCart.Search.Application.Documents;

namespace TechCart.Search.Infrastructure;

// Products indeksi olusturur

//ProductSearchIndex servis olarak kaydettigmiz ElasticsearchClient nesnesini alir ve boylece elk'ya istek gonderebilir

public class ProductSearchIndex(ElasticsearchClient client) : IProductSearchIndex
{
    public const string IndexName = "products";

    public async Task<bool> RecreateAsync(CancellationToken ct)
    {
        // İndeks varsa siler. Yoksa yanıt geçersiz döner, bizi ilgilendirmiyor.
        await client.Indices.DeleteAsync(IndexName, ct);

        // yeni bir indeks olustururuz
        var response = await client.Indices.CreateAsync<ProductSearchDocument>(index => index
            .Index(IndexName)
            .Settings(settings => settings
                    // products indeksimde her urun ayri bir dokuman olacak. 
                .NumberOfShards(1) // tum urunlerin dokumanlarini tek bir ana shard'da tutariz
                .NumberOfReplicas(0) // Bu parcanin elk iicnde ek bir kopyasi/replikasi olusturulmayacak
                .Analysis(analysis => analysis
                            // cozumleme (analyzers) kurallari:
                    .Analyzers(analyzers => analyzers
                        .Custom("folded", custom => custom
                            .Tokenizer("standard") // metni kelimelere ayiri
                            //asciifolding->aksanli kelimeleri sadelestirir, lowercase-> buyuk harfleri kucultur
                            .Filter(new[] { "asciifolding", "lowercase" })))))
            // urun dokumaninda hangi alanlar var her alani nasil kullanacagiz eslemesini yapariz
            .Mappings(mappings => mappings
                 // yeni bir alan tanimlayip elk'ya gonderirsen elk o dokumani reddeder   
                .Dynamic(DynamicMapping.Strict)
                .Properties(properties => properties
                    .Keyword(x => x.Id)
                    // urun adini kelimelere ayrilararak metin aramasina hazirlariz
                    .Text(x => x.Name, text => text.Analyzer("folded"))
                    // kullanici yazarken oneri/ek arama icin uygun alt alanlari olustururuz
                    .SearchAsYouType(x => x.SearchText, sayt => sayt.Analyzer("folded"))
                    .Keyword(x => x.Brand)
                    .Keyword(x => x.Category)
                    .Keyword(x => x.Color)
                    .FloatNumber(x => x.Price)
                    .Boolean(x => x.InStock)
                    .Keyword(x => x.Image!, keyword => keyword.Index(false)) // görsel adresini arama eşleşmesi için indekslemez
                    .Date(x => x.CreatedAt))));

        // yeni indeks olusturma istegi basarili mi
        return response.IsValidResponse;
    }
    
    // birden fazla urunu tek bir istekle toplu olarak elk indeksine yazan fonks.
    // documents-> indekslenecek urun dokumanlari
    public async Task IndexManyAsync(IReadOnlyCollection<ProductSearchDocument> documents, CancellationToken ct)
    {
        if (documents.Count == 0)
            return;

        // bulk istegi hazirlanir. bos bir toplu istek olusturulur.
        // IndexName -> dokumanalrin yazilacagi indeksin adi
        // Operations -> dokuman icindeki yapilacak islemler
        var request = new BulkRequest(IndexName) { Operations = [] };

        // her urun dokumana ekleniyor
        foreach (var document in documents)
            request.Operations.Add(new BulkIndexOperation<ProductSearchDocument>(document) { Id = document.Id });

        // İsteği Elasticsearch’e gönderir
        var response = await client.BulkAsync(request, ct);
        
        if (!response.IsValidResponse || response.Errors)
        {
            var problems = string.Join("; ", response.ItemsWithErrors
                .Take(5)
                .Select(item => $"{item.Id}: {item.Error?.Reason}"));

            throw new InvalidOperationException($"Toplu yazma başarısız: {problems}");
        }
    }
    // Elasticsearch’teki ürün dokümanları arasında arama yapıp sonuçların istenen sayfasını döndürür
    public async Task<ProductSearchPage> SearchAsync(string term, int page, int pageSize, CancellationToken ct)
    {
        var request = new SearchRequest<ProductSearchDocument>(IndexName)
        {
            From = (page - 1) * pageSize,
            Size = pageSize,
            Query = new Query { Bool = new BoolQuery { Must = BuildMustClauses(term) } }
        };

        // Elasticsearch’e arama isteği bu satırda gönderilir
        var response = await client.SearchAsync<ProductSearchDocument>(request, ct);
        if (!response.IsValidResponse)
            throw new InvalidOperationException($"Arama başarısız: {response.DebugInformation}");

        return new ProductSearchPage(response.Documents.ToList(), response.Total);
    }

    // Sorguyu kelime kelime ayırır. Yalnızca rakamlardan oluşan kelimeler tam eşleşme ister; harf içeren kelimeler yazım hatasına
    // toleranslı (fuzzy) aranır. Böylece "250" araması "260"ı getirmez, ama "bluetoth" yine de "bluetooth"u bulur.
    
    
    // bu metor kullancinin yazdigi arama metnini kelimelere ayirip her kelime icin bir elasticsearch arama kosuli haizrlar.
    private static List<Query> BuildMustClauses(string term)
    {
        // metni kelimelere ayirir. kullancii "Samsung S24 256" yazsin.
        // sonu ["Samsung", "S24", "256"] olur
        // RemoveEmptyEntries -> arada birden fazla boşluk varsa boş parçaları atar.
        // TrimEntries ise parçaların başındaki ve sonundaki boşlukları temizler.
        var tokens = term.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        // her kelime icin ayri bir kosul olusturur
        return tokens.Select(token =>
        {
            // kelimenin tamami rakamlardan mi olusuyor diye kontrol eder
            var isNumeric = token.All(char.IsDigit);

            return new Query
            {
                // her kelimeyi urun dokumaninin searchText alaninda arar
                Match = new MatchQuery
                {
                    Field = "searchText",
                    Query = token,
                    Fuzziness = isNumeric ? null : new Fuzziness("AUTO")
                }
            };
        }).ToList();
    }
    
    // bu metot, elasticsearch'te oneriye uygun urunleri arar ve urun adlarindan olusan bir liste doner
    public async Task<IReadOnlyList<string>> SuggestAsync(string term, int limit, CancellationToken ct)
    {
        // elasticsearch'e IndexName adli indekste, ProductSearchDocument turundeki urunleri aramak icin nesne olustururuz
        var request = new SearchRequest<ProductSearchDocument>(IndexName)
        {
            // en fazla 8 urun dokumani getir 
            Size = limit,
            
            // neye gore arayacaginin kuralini belirtiriz
            Query = new Query
            {
                MultiMatch = new MultiMatchQuery
                {
                    Query = term, // kullanicinin yazfigi metni aramaya ver
                    Type = TextQueryType.BoolPrefix,
                    // fields -> bu metni hangi alanlarda araycagimizi souleriz 
                    // searchText alan, _2gram ve _3gram alt alanlar.
                    // Kullanıcının yazdığı metni bu üç arama alanını kullanarak değerlendirir
                    Fields = new[] { "searchText", "searchText._2gram", "searchText._3gram" } 
                    //  Samsung Galaxy S24 duz metin olsun. 
                    // searchText -> Samsung, Galaxy, S24
                    // searchText._2gram -> yan yana iki kelimelik gruplar. Samsung Galaxy, Galaxy S24
                    // searchText._3gram -> Samsung Galaxy S24
                }
            }
        };

        // elasticsarhte arama yapilir. uygun bulunan urun dokumanlari repsone icinde doner
        var response = await client.SearchAsync<ProductSearchDocument>(request, ct);

        // elasticsearch istegi basarisizsa hata firlatir
        if (!response.IsValidResponse)
            throw new InvalidOperationException($"Öneri sorgusu başarısız: {response.DebugInformation}");

        // urunun sadeve isimlerini alir. Distinct ile ayni adin tekrar etmesini onleriz
        return response.Documents.Select(d => d.Name).Distinct().ToList();
    }
    
    // Elasticsearch'teki belirli bir urunun sadece InStock alanini guncelliyor.
    // stok adedi güncellenmiyor sadece ürünün stokta olup olmadığı bilgisi güncelleniyor.
    // Guncellenecek urunu ve yeni stok durumunu alir
    public async Task UpdateStockAsync(Guid productId, bool inStock, CancellationToken ct)
    {
        // guncellemenin hangi indekste o indeksin hangi dokumaninda yapilacagi bulunur
        var response = await client.UpdateAsync<ProductSearchDocument, ProductStockPatch>(
            IndexName, productId.ToString(), u => 
                // Bu dokümanın InStock alanına gelen değer ile guncelle.
                u.Doc(new ProductStockPatch { InStock = inStock }), ct);

        if (!response.IsValidResponse)
            throw new InvalidOperationException($"Stok güncellemesi başarısız: {response.DebugInformation}");
    }

    // Kısmi güncelleme için yalnızca InStock alanını taşır.
    private class ProductStockPatch
    {
        public bool InStock { get; set; }
    }
}