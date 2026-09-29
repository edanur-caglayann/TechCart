using Microsoft.AspNetCore.Mvc;
using TechCart.Products.Application.Dtos.RequestDtos;
using TechCart.Products.Application.ListForIndexing;
using TechCart.Search.Application.Abstractions;
using TechCart.Search.Application.Documents;
using TechCart.Search.Application.Reindex;

namespace TechCart.Api.Controllers.Search;

[ApiController]
[Route("api/search/dev")]
public class SearchDevController(IProductSearchIndex productSearchIndex,
    ListProductsForIndexingHandler listProductsForIndexingHandler,
    ReindexProductsHandler reindexProductsHandler) : ControllerBase
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

    [HttpGet("documents-preview")]
    public async Task<IActionResult> DocumentsPreview(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var items = await listProductsForIndexingHandler.Handle(new ListProductsForIndexingQuery(page, pageSize), ct);
        return Ok(items.Select(ProductSearchDocumentMapper.ToDocument));
    }

    [HttpPost("index-page")]
    public async Task<IActionResult> IndexPage(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 3)
    {
        var items = await listProductsForIndexingHandler.Handle(new ListProductsForIndexingQuery(page, pageSize), ct);
        var documents = items.Select(ProductSearchDocumentMapper.ToDocument).ToList();

        await productSearchIndex.IndexManyAsync(documents, ct);

        return Ok(new { indexed = documents.Count });
    }

    // GEÇİCİ: tüm ürünleri baştan indeksleyen döngüyü tetikler.
    [HttpPost("reindex-all")]
    public async Task<IActionResult> ReindexAll(CancellationToken ct)
    {
        var result = await reindexProductsHandler.Handle(new ReindexProductsCommand(), ct);
        return Ok(result);
    }
}