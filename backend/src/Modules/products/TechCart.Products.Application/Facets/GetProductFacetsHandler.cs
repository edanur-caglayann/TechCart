using TechCart.Brands.Application.Abstractions;
using TechCart.Categories.Application.Abstractions;
using TechCart.Products.Application.Abstractions;
using TechCart.Products.Application.Dtos.RequestDtos;
using TechCart.Products.Contracts.Dtos.ResponseDtos;

namespace TechCart.Products.Application.Facets;

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

    public async Task<ProductFacetsResponse> Handle(GetProductFacetsQuery query, CancellationToken ct)
    {
        var filter = new ProductSearchFilter(query.SearchTerm, query.CategoryId, query.BrandId,
            query.MinPrice, query.MaxPrice, query.Color, query.InStock, SortBy: "relevance", Page: 1, PageSize: 1);

        var categoryFacets = await _productReadRepository.GetCategoryFacetsAsync(filter, ct);
        var brandFacets = await _productReadRepository.GetBrandFacetsAsync(filter, ct);
        var colorFacets = await _productReadRepository.GetColorFacetsAsync(filter, ct);
        var (minPrice, maxPrice) = await _productReadRepository.GetPriceRangeAsync(filter, ct);

        var categoriesById = (await _categoryReadRepository
            .GetByIdsAsync(categoryFacets.Select(f => f.CategoryId), ct)).ToDictionary(c => c.Id);
        var brandsById = (await _brandReadRepository
            .GetByIdsAsync(brandFacets.Select(f => f.BrandId), ct)).ToDictionary(b => b.Id);

        return new ProductFacetsResponse(
            categoryFacets.Select(f => new FacetOptionResponse(f.CategoryId,
                categoriesById.TryGetValue(f.CategoryId, out var c) ? c.Name : "Bilinmiyor", f.Count)).ToList(),
            brandFacets.Select(f => new FacetOptionResponse(f.BrandId,
                brandsById.TryGetValue(f.BrandId, out var b) ? b.Name : "Bilinmiyor", f.Count)).ToList(),
            colorFacets.Select(f => new ColorFacetResponse(f.Color, f.Count)).ToList(),
            minPrice, maxPrice);
    }
}
