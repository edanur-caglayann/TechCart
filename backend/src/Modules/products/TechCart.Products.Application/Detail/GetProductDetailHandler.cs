using TechCart.Brands.Application.Abstractions;
using TechCart.Categories.Application.Abstractions;
using TechCart.ProductImages.Application.Abstractions;
using TechCart.Products.Application.Abstractions;
using TechCart.Products.Domain.Exceptions;

namespace TechCart.Products.Application.Detail;

public record GetProductDetailQuery(Guid ProductId);
public record ProductDetailDto(Guid Id, string Name, string Brand, string Category, string Model,
    string Description, string Specs, decimal Price, List<string> Images);

// tek urunun detaylarini 
public class GetProductDetailHandler
{
    private readonly IProductReadRepository _productReadRepository;
    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly IBrandReadRepository _brandReadRepository;
    private readonly IProductImageReadRepository _productImageReadRepository;

    public GetProductDetailHandler(IProductReadRepository productReadRepository,
        ICategoryReadRepository categoryReadRepository, IBrandReadRepository brandReadRepository,
        IProductImageReadRepository productImageReadRepository)
    {
        _productReadRepository = productReadRepository;
        _categoryReadRepository = categoryReadRepository;
        _brandReadRepository = brandReadRepository;
        _productImageReadRepository = productImageReadRepository;
    }

    public async Task<ProductDetailDto> Handle(GetProductDetailQuery query, CancellationToken ct)
    {
        var product = await _productReadRepository.GetByIdAsync(query.ProductId, ct)
            ?? throw new ProductNotFoundException(query.ProductId);
        
        var category = await _categoryReadRepository.GetByIdsAsync(new[] { product.CategoryId }, ct);
        var brand = await _brandReadRepository.GetByIdsAsync(new[] { product.BrandId }, ct);
        var images = await _productImageReadRepository.GetImagesByProductIdAsync(product.Id, ct);

        return new ProductDetailDto(
            product.Id, product.Name,
            brand.FirstOrDefault()?.Name ?? "Bilinmeyen Marka",
            category.FirstOrDefault()?.Name ?? "Bilinmiyor",
            product.Model, product.Description, product.Specs,
            Math.Round(product.Price * (1 + product.VatRate), 2),
            images);
    }
}