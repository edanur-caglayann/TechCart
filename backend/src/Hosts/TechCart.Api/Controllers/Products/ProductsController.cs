using Microsoft.AspNetCore.Mvc;
using TechCart.Products.Application.Detail;
using TechCart.Products.Application.Facets;
using TechCart.Products.Application.List;
using TechCart.Products.Application.Suggestions;

namespace TechCart.Api.Controllers.Products;

[ApiController]
[Route("api/products")]
// [Authorize] YOK — misafir kullanıcı da ürünleri görebilmeli/arayabilmeli.
public class ProductsController : ControllerBase
{
    private const int MaxPageSize = 100;

    private readonly ListProductsHandler _listProductsHandler;
    private readonly GetProductFacetsHandler _getProductFacetsHandler;
    private readonly GetProductSuggestionsHandler _getProductSuggestionsHandler;
    private readonly GetProductDetailHandler _getProductDetailHandler;

    public ProductsController(ListProductsHandler listProductsHandler, GetProductFacetsHandler getProductFacetsHandler,
        GetProductSuggestionsHandler getProductSuggestionsHandler, GetProductDetailHandler getProductDetailHandler)
    {
        _listProductsHandler = listProductsHandler;
        _getProductFacetsHandler = getProductFacetsHandler;
        _getProductSuggestionsHandler = getProductSuggestionsHandler;
        _getProductDetailHandler = getProductDetailHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductListRequest request, CancellationToken ct)
    {
        // Client aşırı büyük pageSize isterse veritabanını zorlamayalım.
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
        var page = Math.Max(request.Page, 1);

        var query = new ListProductsQuery(request.Q, request.Category, request.Brand,
            request.MinPrice, request.MaxPrice, request.Color, request.SortBy, page, pageSize);

        var result = await _listProductsHandler.Handle(query, ct);
        var totalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize);

        return Ok(new ProductListResponse(
            result.Items.Select(p => new ProductListItemResponse(p.Id, p.Name, p.Brand, p.Price, p.Image)).ToList(),
            new PaginationResponse(page, pageSize, result.TotalCount, totalPages)));
    }

    [HttpGet("facets")]
    public async Task<IActionResult> GetFacets([FromQuery] ProductFacetsRequest request, CancellationToken ct)
    {
        var query = new GetProductFacetsQuery(request.Q, request.Category, request.Brand, request.MinPrice, request.MaxPrice, request.Color);
        var result = await _getProductFacetsHandler.Handle(query, ct);

        return Ok(new ProductFacetsResponse(
            result.Categories.Select(c => new FacetOptionResponse(c.Id, c.Name, c.Count)).ToList(),
            result.Brands.Select(b => new FacetOptionResponse(b.Id, b.Name, b.Count)).ToList(),
            result.Colors.Select(c => new ColorFacetResponse(c.Color, c.Count)).ToList(),
            result.MinPrice, result.MaxPrice));
    }

    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions([FromQuery] string q, CancellationToken ct)
    {
        var result = await _getProductSuggestionsHandler.Handle(new GetProductSuggestionsQuery(q), ct);
        return Ok(result.Select(s => new SuggestionResponse(s.Text, s.Type)));
    }

    // {id:guid} kısıtı sayesinde "facets"/"suggestions" gibi metin route'larla
    // hiç çakışmıyor — ASP.NET Core bunları otomatik ayırt ediyor.
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductDetail(Guid id, CancellationToken ct)
    {
        var result = await _getProductDetailHandler.Handle(new GetProductDetailQuery(id), ct);
        return Ok(new ProductDetailResponse(result.Id, result.Name, result.Brand, result.Category,
            result.Model, result.Description, result.Specs, result.Price, result.Images));
    }
}