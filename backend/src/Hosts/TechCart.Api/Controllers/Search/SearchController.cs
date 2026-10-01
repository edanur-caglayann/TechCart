using Microsoft.AspNetCore.Mvc;
using TechCart.Search.Application.SearchProducts;
using TechCart.Search.Application.Suggest;

namespace TechCart.Api.Controllers.Search;

[ApiController]
[Route("api/search")]
public class SearchController(
    SearchProductsHandler searchProductsHandler,
    SuggestProductsHandler suggestProductsHandler) : ControllerBase
{
    [HttpGet("products")]
    public async Task<IActionResult> SearchProducts(CancellationToken ct, [FromQuery] string q = "",
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await searchProductsHandler.Handle(new SearchProductsQuery(q, page, pageSize), ct);
        return Ok(result);
    }
    
    [HttpGet("suggestions")]
    public async Task<IActionResult> Suggestions(CancellationToken ct, [FromQuery] string q = "",
        [FromQuery] int limit = 8)
    {
        var result = await suggestProductsHandler.Handle(new SuggestProductsQuery(q, limit), ct);
        return Ok(result);
    }
}