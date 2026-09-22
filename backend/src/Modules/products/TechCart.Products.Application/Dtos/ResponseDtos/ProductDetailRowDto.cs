namespace TechCart.Products.Application.Dtos.ResponseDtos;

public record ProductDetailRowDto(Guid Id, Guid CategoryId, Guid BrandId, string Name, string Model,
    string Description, string Specs, decimal Price, decimal VatRate);
