namespace TechCart.Products.Contracts.Dtos.ResponseDtos;

// Search modülünün göreceği, indekslenecek ürün bilgisi.
public record ProductIndexItemResponse(Guid Id, string Name, string Brand, string Category, string Color,
    decimal Price, string? Image, bool InStock, DateTime CreatedAt);