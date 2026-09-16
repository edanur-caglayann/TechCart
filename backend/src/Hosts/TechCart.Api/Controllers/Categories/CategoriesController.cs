using Microsoft.AspNetCore.Mvc;
using TechCart.Categories.Application.List;

namespace TechCart.Api.Controllers.Categories;

[ApiController]
[Route("api/categories")]
// [Authorize] Yok cunku misafir kullanıcı da ürünleri kategoriye göre filtreleyebilmeli
public class CategoriesController : ControllerBase
{
    private readonly ListCategoriesHandler _listCategoriesHandler;
    public CategoriesController(ListCategoriesHandler listCategoriesHandler) => _listCategoriesHandler = listCategoriesHandler;

    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var result = await _listCategoriesHandler.Handle(ct);
        return Ok(result);
    }
}