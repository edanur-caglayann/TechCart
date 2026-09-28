using Microsoft.AspNetCore.Mvc;
using TechCart.Products.Application.Dtos.RequestDtos;
using TechCart.Products.Application.ListForIndexing;
using TechCart.Search.Application.Abstractions;
using TechCart.Search.Application.Documents;

namespace TechCart.Api.Controllers.Search;

// GEÇİCİ: geliştirme sırasında elle tetiklemek için. Yeniden indeksleme komutu
// yazıldığında kaldırılacak.
[ApiController]
[Route("api/search/dev")]
public class SearchDevController(IProductSearchIndex productSearchIndex,
    ListProductsForIndexingHandler listProductsForIndexingHandler) : ControllerBase
{
    [HttpPost("recreate-index")]
    public async Task<IActionResult> RecreateIndex(CancellationToken ct)
    {
        var created = await productSearchIndex.RecreateAsync(ct);
        return Ok(new { created });
    }

    [HttpGet("products-page")]
    public async Task<IActionResult> ProductsPage(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var items = await listProductsForIndexingHandler.Handle(new ListProductsForIndexingQuery(page, pageSize), ct);
        return Ok(items);
    }

    // Products'tan gelen sayfayı, Elasticsearch'e yazılacak doküman hâline çevirip gösterir.
    // Henüz hiçbir şey yazmıyor, yalnızca çeviriyi doğrulamak için.
    [HttpGet("documents-preview")]
    public async Task<IActionResult> DocumentsPreview(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var items = await listProductsForIndexingHandler.Handle(new ListProductsForIndexingQuery(page, pageSize), ct);
        return Ok(items.Select(ProductSearchDocumentMapper.ToDocument));
    }
}