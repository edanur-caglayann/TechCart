using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.Mapping;
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
}