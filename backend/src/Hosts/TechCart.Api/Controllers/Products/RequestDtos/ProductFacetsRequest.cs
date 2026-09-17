namespace TechCart.Api.Controllers.Products.RequestDtos;

public class ProductFacetsRequest
{
    public string? Q { get; set; }
    public Guid? Category { get; set; }
    public Guid? Brand { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Color { get; set; }
    public bool? InStock { get; set; }
}
