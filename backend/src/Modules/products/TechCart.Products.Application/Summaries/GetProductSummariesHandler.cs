using TechCart.Brands.Application.Abstractions;
using TechCart.Categories.Application.Abstractions;
using TechCart.ProductImages.Application.Abstractions;
using TechCart.Products.Application.Abstractions;
using TechCart.Products.Application.Dtos.ResponseDtos;

namespace TechCart.Products.Application.Summaries;

public record GetProductSummariesQuery(List<Guid> ProductIds);

public class GetProductSummariesHandler(
    IProductReadRepository productReadRepository,
    ICategoryReadRepository categoryReadRepository,
    IBrandReadRepository brandReadRepository,
    IProductImageReadRepository productImageReadRepository)
{
    public async Task<List<ProductSummaryDto>> Handle(GetProductSummariesQuery query, CancellationToken ct)
    {
        if (query.ProductIds.Count == 0)
            return [];

        var products = await productReadRepository.GetByIdsAsync(query.ProductIds, ct);

        var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
        var categoriesById = (await categoryReadRepository.GetByIdsAsync(categoryIds, ct)).ToDictionary(c => c.Id);

        var brandIds = products.Select(p => p.BrandId).Distinct().ToList();
        var brandsById = (await brandReadRepository.GetByIdsAsync(brandIds, ct)).ToDictionary(b => b.Id);

        var imagesByProductId = await productImageReadRepository.GetPrimaryImagesByProductIdsAsync(query.ProductIds, ct);

        return products.Select(p => new ProductSummaryDto(
            p.Id, p.Name, p.Model,
            brandsById.TryGetValue(p.BrandId, out var brand) ? brand.Name : "Bilinmeyen Marka",
            categoriesById.TryGetValue(p.CategoryId, out var category) ? category.Name : "Bilinmiyor",
            Math.Round(p.Price * (1 + p.VatRate), 2),
            p.VatRate,
            imagesByProductId.GetValueOrDefault(p.Id),
            p.Stock
        )).ToList();
    }
}