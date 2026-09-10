using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechCart.Users.Application.Auth;
using TechCart.Users.Application.Login;
using TechCart.Users.Application.Profile;
using TechCart.Users.Application.Register;

namespace TechCart.Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{   
    private readonly RegisterUserHandler _registerUserHandler;
    private readonly LoginHandler _loginHandler;
    private readonly GetMyProfileHandler _getMyProfileHandler;

    public AuthController(RegisterUserHandler registerUserHandler, LoginHandler loginHandler,  GetMyProfileHandler getMyProfileHandler)
    {
        _registerUserHandler = registerUserHandler;
        _loginHandler = loginHandler;
        _getMyProfileHandler = getMyProfileHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        // gelen kayit istegini application katmaninin anlayacagi formata donusturur
        var command = new RegisterUserCommand(request.FirstName, request.LastName, request.Email, request.Password);
        // application'a iletilir 
        var result = await _registerUserHandler.Handle(command, ct);
        return Ok(result); // basarili ise 200 durum kodu ve bilgiler doner
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _loginHandler.Handle(command, ct);
        return Ok(result);
    }

  
    [HttpGet("session")]
    [Authorize] // yalnzica gecerli bir JWT gonderen kullanicilarin erisebilmesini saglar
    //istek buraya ulasamadan once JWT kontorlu yapilir 
    public async Task<IActionResult> Session(CancellationToken ct) // artık async, dikkat
    {
        // Sadece kimliği (userId) token'dan alıyoruz diger bilgileri GetMyProfileHandler uzerinden aliriz
        var userId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var profile = await _getMyProfileHandler.Handle(new GetMyProfileQuery(userId), ct);

        var authUser = new AuthUserDto(profile.Id, profile.FirstName, profile.LastName, profile.Email, profile.Role);
        return Ok(new SessionResponse(true, authUser));
    }


    [HttpPost("logout")]
    [Authorize] // yalnizca gecerli token'a sahip kullanicilar cagirabilir
    public IActionResult Logout()
    {
        return NoContent();
    }
}