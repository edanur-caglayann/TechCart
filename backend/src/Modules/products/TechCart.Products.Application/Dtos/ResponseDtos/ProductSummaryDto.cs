namespace TechCart.Products.Application.Dtos.ResponseDtos;

// CartItems gibi başka modüllerin ihtiyacı olan, en yalın ürün özeti
public record ProductSummaryDto(
    Guid Id,
    string Name,
    string Model,
    string Brand,
    string Category,
    decimal Price, // KDV dahil fiyat
    decimal VatRate,
    string? Image,
    int Stock);