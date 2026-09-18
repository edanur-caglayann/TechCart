namespace TechCart.Products.Application.Dtos.ResponseDtos;

public record ProductRowDto(Guid Id, Guid CategoryId, Guid BrandId, string Name, string Model, decimal Price, decimal VatRate, int Stock);
