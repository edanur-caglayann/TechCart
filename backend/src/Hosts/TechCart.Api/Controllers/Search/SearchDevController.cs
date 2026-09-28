using Microsoft.AspNetCore.Mvc;
using TechCart.Search.Application.Abstractions;

namespace TechCart.Api.Controllers.Search;

// GEÇİCİ: geliştirme sırasında elle tetiklemek için. Yeniden indeksleme komutu
// yazıldığında kaldırılacak.
[ApiController]
[Route("api/search/dev")]
public class SearchDevController(IProductSearchIndex productSearchIndex) : ControllerBase
{
    [HttpPost("recreate-index")]
    public async Task<IActionResult> RecreateIndex(CancellationToken ct)
    {
        var created = await productSearchIndex.RecreateAsync(ct);
        return Ok(new { created });
    }
}