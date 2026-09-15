using TechCart.Brands.Application.Abstractions;
using TechCart.ProductImages.Application.Abstractions;
using TechCart.Products.Application.Abstractions;
using TechCart.SharedKernel;

namespace TechCart.Products.Application.List;

public record ListProductsQuery(string? SearchTerm, Guid? CategoryId, Guid? BrandId,
    decimal? MinPrice, decimal? MaxPrice, string? Color, string SortBy, int Page, int PageSize);

public record ProductListItemDto(Guid Id, string Name, string Brand, decimal Price, string? Image);

public class ListProductsHandler
{
    private readonly IProductReadRepository _productReadRepository;
    private readonly IBrandReadRepository _brandReadRepository;
    private readonly IProductImageReadRepository _productImageReadRepository;

    public ListProductsHandler(IProductReadRepository productReadRepository,
        IBrandReadRepository brandReadRepository, IProductImageReadRepository productImageReadRepository)
    {
        _productReadRepository = productReadRepository;
        _brandReadRepository = brandReadRepository;
        _productImageReadRepository = productImageReadRepository;
    }

    public async Task<PagedResult<ProductListItemDto>> Handle(ListProductsQuery query, CancellationToken ct)
    {
        var filter = new ProductSearchFilter(query.SearchTerm, query.CategoryId, query.BrandId,
            query.MinPrice, query.MaxPrice, query.Color, query.SortBy, query.Page, query.PageSize);

        var page = await _productReadRepository.SearchAsync(filter, ct);

        // Sayfadaki ürünlerin marka/görsel bilgisini TOPLU çekiyoruz — 20 ürün
        // için 20 ayrı sorgu değil, sadece 2 ek sorgu (marka + görsel).
        var brandIds = page.Items.Select(p => p.BrandId).Distinct().ToList();
        var brandsById = (await _brandReadRepository.GetByIdsAsync(brandIds, ct)).ToDictionary(b => b.Id);

        var productIds = page.Items.Select(p => p.Id).ToList();
        var imagesByProductId = await _productImageReadRepository.GetPrimaryImagesByProductIdsAsync(productIds, ct);

        var items = page.Items.Select(p => new ProductListItemDto(
            p.Id,
            p.Name,
            brandsById.TryGetValue(p.BrandId, out var brand) ? brand.Name : "Bilinmeyen Marka",
            // KDV DAHİL fiyat — müşterinin ekranda gördüğü ve ödeyeceği tutar.
            Math.Round(p.Price * (1 + p.VatRate), 2),
            imagesByProductId.GetValueOrDefault(p.Id)
        )).ToList();

        return new PagedResult<ProductListItemDto>(items, page.TotalCount, page.PageNumber, page.PageSize);
    }
}