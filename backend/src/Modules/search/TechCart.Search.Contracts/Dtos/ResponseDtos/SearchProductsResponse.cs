namespace TechCart.Search.Contracts.Dtos.ResponseDtos;

public record SearchProductItemResponse(Guid Id, string Name, string Brand, string Category, string Color,
    decimal Price, string? Image, bool InStock);

public record SearchPaginationResponse(int Page, int PageSize, long TotalItems, int TotalPages);

public record SearchProductsResponse(List<SearchProductItemResponse> Items, SearchPaginationResponse Pagination);