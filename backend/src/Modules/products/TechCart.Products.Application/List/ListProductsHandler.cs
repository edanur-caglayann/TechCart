using TechCart.Brands.Application.Abstractions;
using TechCart.Categories.Application.Abstractions;
using TechCart.ProductImages.Application.Abstractions;
using TechCart.Products.Application.Abstractions;
using TechCart.SharedKernel;

namespace TechCart.Products.Application.List;

public record ListProductsQuery(string? SearchTerm, Guid? CategoryId, Guid? BrandId,
    decimal? MinPrice, decimal? MaxPrice, string? Color, bool? InStock,
    string SortBy, int Page, int PageSize);

public record ProductListItemDto(Guid Id, string Name, string Model, string Brand, string Category,
    decimal Price, decimal VatRate, string? Image, bool InStock);

public class ListProductsHandler
{
    private readonly IProductReadRepository _productReadRepository;
    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly IBrandReadRepository _brandReadRepository;
    private readonly IProductImageReadRepository _productImageReadRepository;
    
    public ListProductsHandler(IProductReadRepository productReadRepository,
        ICategoryReadRepository categoryReadRepository, IBrandReadRepository brandReadRepository,
        IProductImageReadRepository productImageReadRepository)
    {
        _productReadRepository = productReadRepository;
        _categoryReadRepository = categoryReadRepository;
        _brandReadRepository = brandReadRepository;
        _productImageReadRepository = productImageReadRepository;
    }

    public async Task<PagedResult<ProductListItemDto>> Handle(ListProductsQuery query, CancellationToken ct)
    {
        var filter = new ProductSearchFilter(query.SearchTerm, query.CategoryId, query.BrandId,
            query.MinPrice, query.MaxPrice, query.Color, query.InStock, query.SortBy, query.Page, query.PageSize);

        var page = await _productReadRepository.SearchAsync(filter, ct);

        var categoryIds = page.Items.Select(p => p.CategoryId).Distinct().ToList();
        var categoriesById = (await _categoryReadRepository.GetByIdsAsync(categoryIds, ct)).ToDictionary(c => c.Id);

        var brandIds = page.Items.Select(p => p.BrandId).Distinct().ToList();
        var brandsById = (await _brandReadRepository.GetByIdsAsync(brandIds, ct)).ToDictionary(b => b.Id);

        var productIds = page.Items.Select(p => p.Id).ToList();
        var imagesByProductId = await _productImageReadRepository.GetPrimaryImagesByProductIdsAsync(productIds, ct);

        var items = page.Items.Select(p => new ProductListItemDto(
            p.Id, p.Name, p.Model,
            brandsById.TryGetValue(p.BrandId, out var brand) ? brand.Name : "Bilinmeyen Marka",
            categoriesById.TryGetValue(p.CategoryId, out var category) ? category.Name : "Bilinmiyor",
            Math.Round(p.Price * (1 + p.VatRate), 2),
            p.VatRate,
            imagesByProductId.GetValueOrDefault(p.Id),
            p.Stock > 0 
        )).ToList();

        return new PagedResult<ProductListItemDto>(items, page.TotalCount, page.PageNumber, page.PageSize);
    }
}
