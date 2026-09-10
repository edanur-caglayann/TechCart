using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechCart.Users.Application.Profile;

namespace TechCart.Api.Controllers.Users;

[ApiController]
[Route("api/users")]
[Authorize] // Controller seviyesinde: buradaki tum endpointler token ister
public class UsersController : ControllerBase
{
    private readonly GetMyProfileHandler _getMyProfileHandler;
    private readonly UpdateProfileHandler _updateProfileHandler;
    private readonly ChangePasswordHandler _changePasswordHandler;

    public UsersController(
        GetMyProfileHandler getMyProfileHandler,
        UpdateProfileHandler updateProfileHandler,
        ChangePasswordHandler changePasswordHandler)
    {
        _getMyProfileHandler = getMyProfileHandler;
        _updateProfileHandler = updateProfileHandler;
        _changePasswordHandler = changePasswordHandler;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var result = await _getMyProfileHandler.Handle(new GetMyProfileQuery(CurrentUserId), ct);
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        var command = new UpdateProfileCommand(CurrentUserId, request.FirstName, request.LastName, request.Email);
        var result = await _updateProfileHandler.Handle(command, ct);
        return Ok(result);
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var command = new ChangePasswordCommand(CurrentUserId, request.CurrentPassword, request.NewPassword);
        await _changePasswordHandler.Handle(command, ct);
        return NoContent();
    }

    // userId asla client'tan alınmaz, her zaman doğrulanmış JWT'nin "sub" claim'inden okunur 
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
}