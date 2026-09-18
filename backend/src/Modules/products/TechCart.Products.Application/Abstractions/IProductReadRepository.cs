using TechCart.SharedKernel;
using TechCart.Products.Application.Dtos.RequestDtos;
using TechCart.Products.Application.Dtos.ResponseDtos;

namespace TechCart.Products.Application.Abstractions;

public interface IProductReadRepository
{
    Task<PagedResult<ProductRowDto>> SearchAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<List<CategoryFacetDto>> GetCategoryFacetsAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<List<BrandFacetDto>> GetBrandFacetsAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<List<ColorFacetDto>> GetColorFacetsAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<(decimal Min, decimal Max)> GetPriceRangeAsync(ProductSearchFilter filter, CancellationToken ct);
    Task<ProductDetailRowDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<string>> SearchProductNamesAsync(string term, int limit, CancellationToken ct);
    
    // cartitem modulu icin -> bir id listesi verilir, o urunlerin satirlarini tek sorguda doner
    Task<List<ProductRowDto>> GetByIdsAsync(List<Guid> ids, CancellationToken ct);
}
