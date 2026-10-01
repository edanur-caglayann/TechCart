using TechCart.Brands.Application.Abstractions;
using TechCart.Categories.Application.Abstractions;
using TechCart.ProductImages.Application.Abstractions;
using TechCart.Products.Application.Abstractions;
using TechCart.Products.Application.Dtos.RequestDtos;
using TechCart.Products.Contracts.Dtos.ResponseDtos;

namespace TechCart.Products.Application.List;

public class ListProductsHandler(
    IProductReadRepository productReadRepository,
    ICategoryReadRepository categoryReadRepository,
    IBrandReadRepository brandReadRepository,
    IProductImageReadRepository productImageReadRepository)
{
    public async Task<ProductListResponse> Handle(ListProductsQuery query, CancellationToken ct)
    {
        
        // Arama terimi bir kategori/marka adıyla da eşleşebilir — örn. "bilgisayar"
        // İngilizce ürün adlarında hiç geçmeyebilir, ama kategori adı Türkçe "Bilgisayar"dır.
        IReadOnlyList<Guid> matchedCategoryIds = [];
        IReadOnlyList<Guid> matchedBrandIds = [];

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var allCategories = await categoryReadRepository.GetAllAsync(ct);
            matchedCategoryIds = allCategories
                .Where(c => c.Name.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Id)
                .ToList();

            var allBrands = await brandReadRepository.GetAllAsync(ct);
            matchedBrandIds = allBrands
                .Where(b => b.Name.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(b => b.Id)
                .ToList();
        }

        var filter = new ProductSearchFilter(query.SearchTerm, query.CategoryId, query.BrandId,
            query.MinPrice, query.MaxPrice, query.Color, query.InStock, query.SortBy, query.Page, query.PageSize,
            matchedCategoryIds, matchedBrandIds);

        var page = await productReadRepository.SearchAsync(filter, ct);
        
        var categoryIds = page.Items.Select(p => p.CategoryId).Distinct().ToList();
        var categoriesById = (await categoryReadRepository.GetByIdsAsync(categoryIds, ct)).ToDictionary(c => c.Id);

        var brandIds = page.Items.Select(p => p.BrandId).Distinct().ToList();
        var brandsById = (await brandReadRepository.GetByIdsAsync(brandIds, ct)).ToDictionary(b => b.Id);

        var productIds = page.Items.Select(p => p.Id).ToList();
        var imagesByProductId = await productImageReadRepository.GetPrimaryImagesByProductIdsAsync(productIds, ct);

        var items = page.Items.Select(p => new ProductListItemResponse(
            p.Id, p.Name, p.Model,
            brandsById.TryGetValue(p.BrandId, out var brand) ? brand.Name : "Bilinmeyen Marka",
            categoriesById.TryGetValue(p.CategoryId, out var category) ? category.Name : "Bilinmiyor",
            Math.Round(p.Price * (1 + p.VatRate), 2),
            p.VatRate,
            imagesByProductId.GetValueOrDefault(p.Id),
            p.Stock > 0
        )).ToList();

        var totalPages = (int)Math.Ceiling(page.TotalCount / (double)page.PageSize);
        var pagination = new PaginationResponse(page.PageNumber, page.PageSize, page.TotalCount, totalPages);

        return new ProductListResponse(items, pagination);
    }
}
