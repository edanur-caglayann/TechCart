using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechCart.Api.Controllers.Cart.Dtos.RequestDtos;
using TechCart.CartItems.Application.Add;
using TechCart.CartItems.Application.Clear;
using TechCart.CartItems.Application.Get;
using TechCart.CartItems.Application.Merge;
using TechCart.CartItems.Application.Remove;
using TechCart.CartItems.Application.Update;

namespace TechCart.Api.Controllers.Cart;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController(GetCartHandler getCartHandler, 
    AddCartItemHandler addCartItemHandler,
    UpdateCartItemQuantityHandler updateCartItemQuantityHandler, 
    RemoveCartItemHandler removeCartItemHandler, 
    ClearCartHandler clearCartHandler,
    MergeCartHandler mergeCartHandler) : ControllerBase{
    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        var result = await getCartHandler.Handle(new GetCartQuery(CurrentUserId), ct);
        return Ok(result);
    }
    
    [HttpPost("add-item")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request, CancellationToken ct)
    {
        var command = new AddCartItemCommand(CurrentUserId, request.ProductId, request.Quantity);
        var result = await addCartItemHandler.Handle(command, ct);
        return Ok(result);
    }
    
    [HttpPatch("items/{productId:guid}")]
    public async Task<IActionResult> UpdateItemQuantity(Guid productId, [FromBody] UpdateCartItemRequest request, CancellationToken ct)
    {
        var command = new UpdateCartItemQuantityCommand(CurrentUserId, productId, request.Quantity);
        var result = await updateCartItemQuantityHandler.Handle(command, ct);
        return Ok(result);
    }
    
    [HttpDelete("items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid productId, CancellationToken ct)
    {
        var command = new RemoveCartItemCommand(CurrentUserId, productId);
        var result = await removeCartItemHandler.Handle(command, ct);
        return Ok(result);
    }

    [HttpDelete("items")]
    public async Task<IActionResult> ClearCart(CancellationToken ct)
    {
        var result = await clearCartHandler.Handle(new ClearCartCommand(CurrentUserId), ct);
        return Ok(result);
    }
    [HttpPost("merge")]
    public async Task<IActionResult> MergeCart([FromBody] MergeCartRequest request, CancellationToken ct)
    {
        var items = request.Items.Select(i => new MergeCartItem(i.ProductId, i.Quantity)).ToList();
        var result = await mergeCartHandler.Handle(new MergeCartCommand(CurrentUserId, items), ct);
        return Ok(result);
    }
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
}