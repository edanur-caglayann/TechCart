using Microsoft.AspNetCore.Mvc;
using TechCart.Search.Application.SearchProducts;

namespace TechCart.Api.Controllers.Search;

[ApiController]
[Route("api/search")]
public class SearchController(SearchProductsHandler searchProductsHandler) : ControllerBase
{
    [HttpGet("products")]
    public async Task<IActionResult> SearchProducts(CancellationToken ct, [FromQuery] string q = "",
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await searchProductsHandler.Handle(new SearchProductsQuery(q, page, pageSize), ct);
        return Ok(result);
    }
}