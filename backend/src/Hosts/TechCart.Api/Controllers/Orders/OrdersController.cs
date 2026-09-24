using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechCart.Api.Controllers.Orders.Dtos.RequestDtos;
using TechCart.Orders.Application.CreateOrder;
using TechCart.Orders.Application.Dtos.RequestDtos;
using TechCart.Orders.Application.ListMyOrders;
using TechCart.Payments.Application.Dtos.RequestDtos;
using TechCart.Payments.Application.InitiateCheckout;

namespace TechCart.Api.Controllers.Orders;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(
    CreateOrderHandler createOrderHandler,
    InitiateCheckoutHandler initiateCheckoutHandler,
    ListMyOrdersHandler listMyOrdersHandler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var command = new CreateOrderCommand(CurrentUserId, request.AddressId);
        var result = await createOrderHandler.Handle(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    
    // belirli bir siparis icin iyzico odeme surecini baslatir
    [HttpPost("{orderId:guid}/checkout")]
    public async Task<IActionResult> Checkout(Guid orderId, CancellationToken ct)
    {
        var command = new InitiateCheckoutCommand(orderId, CurrentUserId);
        var result = await initiateCheckoutHandler.Handle(command, ct);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> ListMyOrders(CancellationToken ct)
    {
        var result = await listMyOrdersHandler.Handle(new ListMyOrdersQuery(CurrentUserId), ct);
        return Ok(result);
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
}