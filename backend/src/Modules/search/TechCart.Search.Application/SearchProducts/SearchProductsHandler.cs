using TechCart.Search.Application.Abstractions;
using TechCart.Search.Contracts.Dtos.ResponseDtos;

namespace TechCart.Search.Application.SearchProducts;

public record SearchProductsQuery(string Term, int Page = 1, int PageSize = 20);

// Api'ye gelen arama istegini duzenleyip elasticsearch aramasina iletir. 
// Elasticsearch'ten gelen sonucu frontend'in kullanacagi bicime cevirir
public class SearchProductsHandler(IProductSearchIndex productSearchIndex)
{
    public async Task<SearchProductsResponse> Handle(SearchProductsQuery query, CancellationToken ct)
    {
        // Elasticsearch'e cok buyuk istekler gitmesin diye kontrol yapariz
        // kullanicidan gelen sayfa num ve sayfa basina urun sayisi gecerli mi diye kontrol ederiz
        var page = query.Page < 1 ? 1 : query.Page; // istenen sayfa 1'den kucukse 1 yap, degilse gonderilen degeri kullan
        // istenen sayfa basina urun sayisi 1-100 arasindaysa onu kullan degilse 20 kullan 
        var pageSize = query.PageSize is < 1 or > 100 ? 20 : query.PageSize;

        // Kullanıcı hiçbir şey yazmamışsa veya sadece boşluk göndermişse Elasticsearch’e istek atılmaz
        if (string.IsNullOrWhiteSpace(query.Term))
            return new SearchProductsResponse([], new SearchPaginationResponse(page, pageSize, 0, 0));

        // SearchAsync fonksiyonu ile elasticsearch aramasini cagirir. ilgili sayfada, ilgili adet kadar urunu ve ilgili terimi bbirlikte arariz
        // Trim() bastaki ve sondaki bosluklari kaldiri
        var result = await productSearchIndex.SearchAsync(query.Term.Trim(), page, pageSize, ct);

        // ilgili arama sonucunda bulunan dokumanlari frontende donecek urune ceviir
        var items = result.Items.Select(d => new SearchProductItemResponse(
            Guid.Parse(d.Id), d.Name, d.Brand, d.Category, d.Color, d.Price, d.Image, d.InStock)).ToList();

        // toplam sayfa sayisi hesaplanir. TotalCount -> aramyla eslesen toplam urun sayisi
        var totalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize);

        return new SearchProductsResponse(items, new SearchPaginationResponse(page, pageSize, result.TotalCount, totalPages));
    }
}