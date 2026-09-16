using TechCart.Brands.Application.Abstractions;
using TechCart.Categories.Application.Abstractions;
using TechCart.Inventory.Application.Abstractions; // YENİ
using TechCart.ProductImages.Application.Abstractions;
using TechCart.Products.Application.Abstractions;
using TechCart.Products.Contracts;
using TechCart.Products.Domain.Exceptions;

namespace TechCart.Products.Application.Detail;

public record GetProductDetailQuery(Guid ProductId);

public class GetProductDetailHandler
{
    private readonly IProductReadRepository _productReadRepository;
    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly IBrandReadRepository _brandReadRepository;
    private readonly IProductImageReadRepository _productImageReadRepository;
    private readonly IProductStockReadRepository _productStockReadRepository; 

    public GetProductDetailHandler(
        IProductReadRepository productReadRepository,
        ICategoryReadRepository categoryReadRepository,
        IBrandReadRepository brandReadRepository,
        IProductImageReadRepository productImageReadRepository,
        IProductStockReadRepository productStockReadRepository) 
    {
        _productReadRepository = productReadRepository;
        _categoryReadRepository = categoryReadRepository;
        _brandReadRepository = brandReadRepository;
        _productImageReadRepository = productImageReadRepository;
        _productStockReadRepository = productStockReadRepository;
    }

    public async Task<ProductDetailResponse> Handle(GetProductDetailQuery query, CancellationToken ct)
    {
        var product = await _productReadRepository.GetByIdAsync(query.ProductId, ct)
            ?? throw new ProductNotFoundException(query.ProductId);

        // urunun icindeki categoryId ve BrandId kullanilarak kategori ve marka bilgisi alinir
        var category = await _categoryReadRepository.GetByIdsAsync(new[] { product.CategoryId }, ct);
        var brand = await _brandReadRepository.GetByIdsAsync(new[] { product.BrandId }, ct);
        var images = await _productImageReadRepository.GetImagesByProductIdAsync(product.Id, ct);
        
        // kdv hesaplanir
        var priceWithoutVat = product.Price;
        var vatAmount = Math.Round(product.Price * product.VatRate, 2);
        var priceWithVat = priceWithoutVat + vatAmount;

        // stok bilgisini alir
        var stockInfo = await _productStockReadRepository.GetByProductIdAsync(product.Id, ct)
            ?? new ProductStockDto(Stock: 0, InStock: false, IsReadyToShip: false, HasFastDelivery: false);

        return new ProductDetailResponse(
            product.Id, product.Name,
            brand.FirstOrDefault()?.Name ?? "Bilinmeyen Marka",
            category.FirstOrDefault()?.Name ?? "Bilinmiyor",
            product.Model, product.Description, product.Specs,
            priceWithVat, priceWithoutVat, product.VatRate, vatAmount,
            stockInfo.Stock, stockInfo.InStock,
            CartQuantity: 0,
            stockInfo.IsReadyToShip, stockInfo.HasFastDelivery,
            images);
    }
}
