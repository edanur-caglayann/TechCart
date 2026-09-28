using TechCart.Brands.Application.Abstractions;
using TechCart.Categories.Application.Abstractions;
using TechCart.ProductImages.Application.Abstractions;
using TechCart.Products.Application.Abstractions;
using TechCart.Products.Application.Dtos.RequestDtos;
using TechCart.Products.Contracts.Dtos.ResponseDtos;

namespace TechCart.Products.Application.ListForIndexing;

// repository aracailgi ile db'den ham urun satirlarini alir ve ProductIndexItemResponse olarak doner
public class ListProductsForIndexingHandler(
    IProductReadRepository productReadRepository,
    ICategoryReadRepository categoryReadRepository,
    IBrandReadRepository brandReadRepository,
    IProductImageReadRepository productImageReadRepository)
{
    public async Task<List<ProductIndexItemResponse>> Handle(ListProductsForIndexingQuery query, CancellationToken ct)
    {
        var rows = await productReadRepository.GetPageForIndexingAsync(query.Page, query.PageSize, ct);

        if (rows.Count == 0)
            return [];

        // sayfadaki tüm ürünlerin kategori, marka ve görsel bilgisi ürün başına ayrı ayrı değil, tek sorguyla çekilir.
        var categoryIds = rows.Select(p => p.CategoryId).Distinct().ToList();
        var categoriesById = (await categoryReadRepository.GetByIdsAsync(categoryIds, ct)).ToDictionary(c => c.Id);

        var brandIds = rows.Select(p => p.BrandId).Distinct().ToList();
        var brandsById = (await brandReadRepository.GetByIdsAsync(brandIds, ct)).ToDictionary(b => b.Id);

        var productIds = rows.Select(p => p.Id).ToList();
        var imagesByProductId = await productImageReadRepository.GetPrimaryImagesByProductIdsAsync(productIds, ct);

        return rows.Select(p => new ProductIndexItemResponse(
            p.Id,
            p.Name,
            brandsById.TryGetValue(p.BrandId, out var brand) ? brand.Name : "Bilinmeyen Marka",
            categoriesById.TryGetValue(p.CategoryId, out var category) ? category.Name : "Bilinmiyor",
            p.Color,
            Math.Round(p.Price * (1 + p.VatRate), 2),
            imagesByProductId.GetValueOrDefault(p.Id),
            p.Stock > 0,
            p.CreatedAt)).ToList();
    }
}