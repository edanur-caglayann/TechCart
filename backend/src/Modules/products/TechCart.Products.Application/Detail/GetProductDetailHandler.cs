using TechCart.Brands.Application.Abstractions;
using TechCart.Categories.Application.Abstractions;
using TechCart.Inventory.Application.Abstractions; // YENİ
using TechCart.Inventory.Application.Dtos.ResponseDtos;
using TechCart.ProductImages.Application.Abstractions;
using TechCart.Products.Application.Abstractions;
using TechCart.Products.Application.Dtos.RequestDtos;
using TechCart.Products.Contracts.Dtos.ResponseDtos;
using TechCart.Products.Domain.Exceptions;

namespace TechCart.Products.Application.Detail;

public class GetProductDetailHandler(
    IProductReadRepository productReadRepository,
    ICategoryReadRepository categoryReadRepository,
    IBrandReadRepository brandReadRepository,
    IProductImageReadRepository productImageReadRepository,
    IProductStockReadRepository productStockReadRepository)
{
    public async Task<ProductDetailResponse> Handle(GetProductDetailQuery query, CancellationToken ct)
    {
        var product = await productReadRepository.GetByIdAsync(query.ProductId, ct)
            ?? throw new ProductNotFoundException(query.ProductId);

        // urunun icindeki categoryId ve BrandId kullanilarak kategori ve marka bilgisi alinir
        var category = await categoryReadRepository.GetByIdsAsync(new[] { product.CategoryId }, ct);
        var brand = await brandReadRepository.GetByIdsAsync(new[] { product.BrandId }, ct);
        var images = await productImageReadRepository.GetImagesByProductIdAsync(product.Id, ct);
        
        // kdv hesaplanir
        var priceWithoutVat = product.Price;
        var vatAmount = Math.Round(product.Price * product.VatRate, 2);
        var priceWithVat = priceWithoutVat + vatAmount;

        // stok bilgisini alir
        var stockInfo = await productStockReadRepository.GetByProductIdAsync(product.Id, ct)
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
