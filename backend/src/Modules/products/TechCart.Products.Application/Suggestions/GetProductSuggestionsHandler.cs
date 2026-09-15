using TechCart.Brands.Application.Abstractions;
using TechCart.Categories.Application.Abstractions;
using TechCart.Products.Application.Abstractions;

namespace TechCart.Products.Application.Suggestions;

public record GetProductSuggestionsQuery(string SearchTerm);
public record SuggestionDto(string Text, string Type); // Type: "product" | "category" | "brand"

// arama onerilerini getirir
public class GetProductSuggestionsHandler
{
    private const int LimitPerType = 5;

    private readonly IProductReadRepository _productReadRepository;
    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly IBrandReadRepository _brandReadRepository;

    public GetProductSuggestionsHandler(IProductReadRepository productReadRepository,
        ICategoryReadRepository categoryReadRepository, IBrandReadRepository brandReadRepository)
    {
        _productReadRepository = productReadRepository;
        _categoryReadRepository = categoryReadRepository;
        _brandReadRepository = brandReadRepository;
    }

    public async Task<List<SuggestionDto>> Handle(GetProductSuggestionsQuery query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query.SearchTerm))
            return new List<SuggestionDto>();

        // Üç kaynağı paralel sorguluyoruz — sırayla çağırsak üç bekleme süresi
        // toplanırdı, Task.WhenAll aynı anda çalıştırıp toplam süreyi kısaltır.
        var productNamesTask = _productReadRepository.SearchProductNamesAsync(query.SearchTerm, LimitPerType, ct);
        var categoryNamesTask = _categoryReadRepository.SearchByNameAsync(query.SearchTerm, LimitPerType, ct);
        var brandNamesTask = _brandReadRepository.SearchByNameAsync(query.SearchTerm, LimitPerType, ct);

        await Task.WhenAll(productNamesTask, categoryNamesTask, brandNamesTask);

        var suggestions = new List<SuggestionDto>();
        suggestions.AddRange(productNamesTask.Result.Select(n => new SuggestionDto(n, "product")));
        suggestions.AddRange(categoryNamesTask.Result.Select(n => new SuggestionDto(n, "category")));
        suggestions.AddRange(brandNamesTask.Result.Select(n => new SuggestionDto(n, "brand")));

        return suggestions;
    }
}