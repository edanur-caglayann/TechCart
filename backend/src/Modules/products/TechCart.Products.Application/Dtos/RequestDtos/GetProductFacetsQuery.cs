namespace TechCart.Products.Application.Dtos.RequestDtos;

public record GetProductFacetsQuery(string? SearchTerm, Guid? CategoryId, Guid? BrandId,
    decimal? MinPrice, decimal? MaxPrice, string? Color, bool? InStock);
