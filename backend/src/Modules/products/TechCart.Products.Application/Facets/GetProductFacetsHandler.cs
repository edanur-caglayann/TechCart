using TechCart.Brands.Application.Abstractions;
using TechCart.Categories.Application.Abstractions;
using TechCart.Products.Application.Abstractions;

namespace TechCart.Products.Application.Facets;

// kullanicidan gelen filtreleme istegi
public record GetProductFacetsQuery(string? SearchTerm, Guid? CategoryId, Guid? BrandId,
    decimal? MinPrice, decimal? MaxPrice, string? Color);

public record FacetOptionDto(Guid Id, string Name, int Count);
public record ColorFacetOptionDto(string Color, int Count);
public record ProductFacetsDto(List<FacetOptionDto> Categories, List<FacetOptionDto> Brands,
    List<ColorFacetOptionDto> Colors, decimal MinPrice, decimal MaxPrice);

// filtre seceneklerini ve sayilarini 
public class GetProductFacetsHandler
{
    private readonly IProductReadRepository _productReadRepository;
    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly IBrandReadRepository _brandReadRepository;

    public GetProductFacetsHandler(IProductReadRepository productReadRepository,
        ICategoryReadRepository categoryReadRepository, IBrandReadRepository brandReadRepository)
    {
        _productReadRepository = productReadRepository;
        _categoryReadRepository = categoryReadRepository;
        _brandReadRepository = brandReadRepository;
    }

    public async Task<ProductFacetsDto> Handle(GetProductFacetsQuery query, CancellationToken ct)
    {
        var filter = new ProductSearchFilter(query.SearchTerm, query.CategoryId, query.BrandId,
            query.MinPrice, query.MaxPrice, query.Color, SortBy: "relevance", Page: 1, PageSize: 1);

        var categoryFacets = await _productReadRepository.GetCategoryFacetsAsync(filter, ct);
        var brandFacets = await _productReadRepository.GetBrandFacetsAsync(filter, ct);
        var colorFacets = await _productReadRepository.GetColorFacetsAsync(filter, ct);
        var (minPrice, maxPrice) = await _productReadRepository.GetPriceRangeAsync(filter, ct);

        var categoriesById = (await _categoryReadRepository
            .GetByIdsAsync(categoryFacets.Select(f => f.CategoryId), ct)).ToDictionary(c => c.Id);
        var brandsById = (await _brandReadRepository
            .GetByIdsAsync(brandFacets.Select(f => f.BrandId), ct)).ToDictionary(b => b.Id);

        return new ProductFacetsDto(
            categoryFacets.Select(f => new FacetOptionDto(f.CategoryId,
                categoriesById.TryGetValue(f.CategoryId, out var c) ? c.Name : "Bilinmiyor", f.Count)).ToList(),
            brandFacets.Select(f => new FacetOptionDto(f.BrandId,
                brandsById.TryGetValue(f.BrandId, out var b) ? b.Name : "Bilinmiyor", f.Count)).ToList(),
            colorFacets.Select(f => new ColorFacetOptionDto(f.Color, f.Count)).ToList(),
            minPrice, maxPrice);
    }
}