using TechCart.Search.Application.Abstractions;
using TechCart.Search.Contracts.Dtos.ResponseDtos;

namespace TechCart.Search.Application.Suggest;

// arama kutusunun altinda gorunen urun adi onerilerini hazirlar

// Term -> kullanicinin yazdigi metin
// limit -> en fazla kac urun sonucunun listelenecegi
public record SuggestProductsQuery(string Term, int Limit = 8);

public class SuggestProductsHandler(IProductSearchIndex productSearchIndex)
{
    public async Task<SearchSuggestionsResponse> Handle(SuggestProductsQuery query, CancellationToken ct)
    {
        // En az 2 harf şartı. 2 karakterden kisaysa elasticsearch'e hic istek atilmaz
        if (string.IsNullOrWhiteSpace(query.Term) || query.Term.Trim().Length < 2)
            return new SearchSuggestionsResponse([]);

        var limit = query.Limit is < 1 or > 20 ? 8 : query.Limit;

        // metni temizleyip asil elasticsearch sorgusuna gonderir
        var suggestions = await productSearchIndex.SuggestAsync(query.Term.Trim(), limit, ct);
        return new SearchSuggestionsResponse(suggestions.ToList());
    }
}