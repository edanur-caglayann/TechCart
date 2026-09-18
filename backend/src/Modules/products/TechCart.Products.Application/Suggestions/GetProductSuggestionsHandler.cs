using TechCart.Brands.Application.Abstractions;
using TechCart.Categories.Application.Abstractions;
using TechCart.Products.Application.Abstractions;
using TechCart.Products.Application.Dtos.RequestDtos;
using TechCart.Products.Contracts.Dtos.ResponseDtos;

namespace TechCart.Products.Application.Suggestions;

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

    public async Task<List<SuggestionResponse>> Handle(GetProductSuggestionsQuery query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query.SearchTerm))
            return new List<SuggestionResponse>();

        // Üç kaynağı paralel sorguluyoruz — sırayla çağırsak üç bekleme süresi
        // toplanırdı, Task.WhenAll aynı anda çalıştırıp toplam süreyi kısaltır.
        var productNamesTask = _productReadRepository.SearchProductNamesAsync(query.SearchTerm, LimitPerType, ct);
        var categoryNamesTask = _categoryReadRepository.SearchByNameAsync(query.SearchTerm, LimitPerType, ct);
        var brandNamesTask = _brandReadRepository.SearchByNameAsync(query.SearchTerm, LimitPerType, ct);

        await Task.WhenAll(productNamesTask, categoryNamesTask, brandNamesTask);

        var suggestions = new List<SuggestionResponse>();
        suggestions.AddRange(productNamesTask.Result.Select(n => new SuggestionResponse(n, "product")));
        suggestions.AddRange(categoryNamesTask.Result.Select(n => new SuggestionResponse(n, "category")));
        suggestions.AddRange(brandNamesTask.Result.Select(n => new SuggestionResponse(n, "brand")));

        return suggestions;
    }
}
