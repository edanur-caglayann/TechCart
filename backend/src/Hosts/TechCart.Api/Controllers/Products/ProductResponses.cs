namespace TechCart.Api.Controllers.Products;

public record ProductListItemResponse(Guid Id, string Name, string Brand, string Category, decimal Price, string? Image, bool InStock);
public record PaginationResponse(int Page, int PageSize, int TotalItems, int TotalPages);
public record ProductListResponse(List<ProductListItemResponse> Items, PaginationResponse Pagination);

public record FacetOptionResponse(Guid Id, string Name, int Count);
public record ColorFacetResponse(string Color, int Count);
public record ProductFacetsResponse(List<FacetOptionResponse> Categories, List<FacetOptionResponse> Brands,
    List<ColorFacetResponse> Colors, decimal MinPrice, decimal MaxPrice);

public record SuggestionResponse(string Text, string Type);

public record ProductDetailResponse(
    Guid Id, string Name, string Brand, string Category, string Model, string Description, string Specs,
    decimal Price, decimal PriceWithoutVat, decimal VatRate, decimal VatAmount,
    int Stock, bool InStock, int CartQuantity,
    bool IsReadyToShip, bool HasFastDelivery,
    List<string> Images);