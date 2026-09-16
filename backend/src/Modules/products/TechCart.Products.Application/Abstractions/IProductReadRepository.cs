using TechCart.SharedKernel;

namespace TechCart.Products.Application.Abstractions;

public record ProductSearchFilter(
    string? SearchTerm, Guid? CategoryId, Guid? BrandId,
    decimal? MinPrice, decimal? MaxPrice, string? Color, bool? InStock,
    string SortBy, int Page, int PageSize);

public record ProductRowDto(Guid Id, Guid CategoryId, Guid BrandId, string Name, string Model, decimal Price, decimal VatRate, int Stock);

public record ProductDetailRowDto(Guid Id, Guid CategoryId, Guid BrandId, string Name, string Model,
    string Description, string Specs, decimal Price, decimal VatRate);

public record CategoryFacetDto(Guid CategoryId, int Count);
public record BrandFacetDto(Guid BrandId, int Count);
public record ColorFacetDto(string Color, int Count);

public interface IProductReadRepository
{
    Task<PagedResult<ProductRowDto>> SearchAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<List<CategoryFacetDto>> GetCategoryFacetsAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<List<BrandFacetDto>> GetBrandFacetsAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<List<ColorFacetDto>> GetColorFacetsAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<(decimal Min, decimal Max)> GetPriceRangeAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<ProductDetailRowDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<string>> SearchProductNamesAsync(string term, int limit, CancellationToken ct);
}