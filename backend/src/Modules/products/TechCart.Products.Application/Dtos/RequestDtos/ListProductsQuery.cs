namespace TechCart.Products.Application.Dtos.RequestDtos;

public record ListProductsQuery(string? SearchTerm, Guid? CategoryId, Guid? BrandId,
    decimal? MinPrice, decimal? MaxPrice, string? Color, bool? InStock,
    string SortBy, int Page, int PageSize);
