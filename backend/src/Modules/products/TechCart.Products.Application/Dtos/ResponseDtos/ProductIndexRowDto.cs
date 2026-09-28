namespace TechCart.Products.Application.Dtos.ResponseDtos;

// İndeksleme için gereken ham alanlar. 
public record ProductIndexRowDto(Guid Id, Guid CategoryId, Guid BrandId, string Name, string Color,
    decimal Price, decimal VatRate, int Stock, DateTime CreatedAt);