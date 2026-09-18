namespace TechCart.Api.Controllers.Products.Dtos.RequestDtos;

public class ProductListRequest
{
    public string? Q { get; set; }
    public Guid? Category { get; set; }
    public Guid? Brand { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Color { get; set; }
    public bool? InStock { get; set; }
    public string SortBy { get; set; } = "relevance";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
