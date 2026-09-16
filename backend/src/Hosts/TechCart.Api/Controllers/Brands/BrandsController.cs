using Microsoft.AspNetCore.Mvc;
using TechCart.Brands.Application.List;

namespace TechCart.Api.Controllers.Brands;

[ApiController]
[Route("api/brands")]
// Categories'teki gibi [Authorize] yok — misafir de markaya göre filtreleyebilmeli.
public class BrandsController : ControllerBase
{
    private readonly ListBrandsHandler _listBrandsHandler;
    public BrandsController(ListBrandsHandler listBrandsHandler) => _listBrandsHandler = listBrandsHandler;

    [HttpGet]
    public async Task<IActionResult> GetBrands(CancellationToken ct)
    {
        var result = await _listBrandsHandler.Handle(ct);
        return Ok(result);
    }
}