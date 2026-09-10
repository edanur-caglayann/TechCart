using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechCart.Addresses.Application.Create;
using TechCart.Addresses.Application.Delete;
using TechCart.Addresses.Application.List;
using TechCart.Addresses.Application.SetDefault;
using TechCart.Addresses.Application.Update;

namespace TechCart.Api.Controllers.Addresses;

[ApiController]
[Route("api/users/me/addresses")]
[Authorize] // bu controller'daki her şey giriş gerektirir
public class AddressesController : ControllerBase
{
    private readonly ListMyAddressesHandler _listMyAddressesHandler;
    private readonly CreateAddressHandler _createAddressHandler;
    private readonly UpdateAddressHandler _updateAddressHandler;
    private readonly DeleteAddressHandler _deleteAddressHandler;
    private readonly SetDefaultAddressHandler _setDefaultAddressHandler;

    public AddressesController(
        ListMyAddressesHandler listMyAddressesHandler,
        CreateAddressHandler createAddressHandler,
        UpdateAddressHandler updateAddressHandler,
        DeleteAddressHandler deleteAddressHandler,
        SetDefaultAddressHandler setDefaultAddressHandler)
    {
        _listMyAddressesHandler = listMyAddressesHandler;
        _createAddressHandler = createAddressHandler;
        _updateAddressHandler = updateAddressHandler;
        _deleteAddressHandler = deleteAddressHandler;
        _setDefaultAddressHandler = setDefaultAddressHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyAddresses(CancellationToken ct)
    {
        var result = await _listMyAddressesHandler.Handle(new ListMyAddressesQuery(CurrentUserId), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAddress([FromBody] AddressRequest request, CancellationToken ct)
    {
        var command = new CreateAddressCommand(CurrentUserId, request.Title, request.FullName,
            request.Phone, request.City, request.District, request.Neighborhood, request.AddressLine, request.PostalCode);
        var result = await _createAddressHandler.Handle(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPatch("{addressId:guid}")]
    public async Task<IActionResult> UpdateAddress(Guid addressId, [FromBody] AddressRequest request, CancellationToken ct)
    {
        var command = new UpdateAddressCommand(addressId, CurrentUserId, request.Title, request.FullName,
            request.Phone, request.City, request.District, request.Neighborhood, request.AddressLine, request.PostalCode);
        var result = await _updateAddressHandler.Handle(command, ct);
        return Ok(result);
    }

    [HttpDelete("{addressId:guid}")]
    public async Task<IActionResult> DeleteAddress(Guid addressId, CancellationToken ct)
    {
        await _deleteAddressHandler.Handle(new DeleteAddressCommand(addressId, CurrentUserId), ct);
        return NoContent();
    }

    [HttpPatch("{addressId:guid}/default")]
    public async Task<IActionResult> SetDefaultAddress(Guid addressId, CancellationToken ct)
    {
        var result = await _setDefaultAddressHandler.Handle(new SetDefaultAddressCommand(addressId, CurrentUserId), ct);
        return Ok(result);
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
}