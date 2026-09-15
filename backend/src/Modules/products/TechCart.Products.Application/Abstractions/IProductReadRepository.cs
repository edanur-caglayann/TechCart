using TechCart.SharedKernel;

namespace TechCart.Products.Application.Abstractions;

// Liste/facet/fiyat aralığı sorgularının HEPSİNİN kullandığı ortak filtre seti.
public record ProductSearchFilter(
    string? SearchTerm, Guid? CategoryId, Guid? BrandId,
    decimal? MinPrice, decimal? MaxPrice, string? Color,
    string SortBy, int Page, int PageSize);

public record ProductRowDto(Guid Id, Guid BrandId, string Name, decimal Price, decimal VatRate);
public record ProductDetailRowDto(Guid Id, Guid CategoryId, Guid BrandId, string Name, string Model,
    string Description, string Specs, decimal Price, decimal VatRate);

public record CategoryFacetDto(Guid CategoryId, int Count);
public record BrandFacetDto(Guid BrandId, int Count);
public record ColorFacetDto(string Color, int Count);

public interface IProductReadRepository
{
    Task<PagedResult<ProductRowDto>> SearchAsync(ProductSearchFilter filter, CancellationToken ct);

    // Facet metotlarının her biri kendi boyutunu (category/brand/color) filtreden
    // cikarir, diğer tüm filtreleri uygular. Filtre secenekleri hazirlanirken kendisini gecici olarak kullanmaz.
    //  böylece kullanıcı bir kategoriye geçtiğinde, o kategoride hangi markaların olduğunu görebilir; kendi
    // kategori seçimi facet'i tek bir sonuca kilitlemez. Kategori: Telefon
    // Marka: Samsung Telefon filtresini kullanır ama Samsung filtresini geçici olarak kullanmaz.
    // Samsung (10), Apple (8) , Xiaomi (5)
    Task<List<CategoryFacetDto>> GetCategoryFacetsAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<List<BrandFacetDto>> GetBrandFacetsAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<List<ColorFacetDto>> GetColorFacetsAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<(decimal Min, decimal Max)> GetPriceRangeAsync(ProductSearchFilter filter, CancellationToken ct);

    Task<ProductDetailRowDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<string>> SearchProductNamesAsync(string term, int limit, CancellationToken ct);
}