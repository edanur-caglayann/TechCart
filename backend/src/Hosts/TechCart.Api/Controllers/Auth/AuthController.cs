using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechCart.Api.Controllers.Auth.RequestDtos;
using TechCart.Users.Application.Auth;
using TechCart.Users.Application.Auth.RequestDtos;
using TechCart.Users.Application.Login;
using TechCart.Users.Application.Login.RequestDtos;
using TechCart.Users.Application.Register;
using TechCart.Users.Application.Register.RequestDtos;

namespace TechCart.Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{   
    private readonly RegisterUserHandler _registerUserHandler;
    private readonly LoginHandler _loginHandler;
    private readonly GetSessionHandler _getSessionHandler;
    private readonly LogoutHandler _logoutHandler;

    public AuthController(
        RegisterUserHandler registerUserHandler,
        LoginHandler loginHandler,
        GetSessionHandler getSessionHandler,
        LogoutHandler logoutHandler)
    {
        _registerUserHandler = registerUserHandler;
        _loginHandler = loginHandler;
        _getSessionHandler = getSessionHandler;
        _logoutHandler = logoutHandler;
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
        // Sadece kimliği (userId) token'dan alıyoruz; response mapping application katmanında yapılır.
        var userId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var result = await _getSessionHandler.Handle(new GetSessionQuery(userId), ct);
        return Ok(result);
    }


    [HttpPost("logout")]
    [Authorize] // yalnizca gecerli token'a sahip kullanicilar cagirabilir
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti)!;
        var exp = long.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Exp)!);
        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;

        await _logoutHandler.Handle(new LogoutCommand(jti, userId, expiresAt), ct);

        return NoContent();
    }
}
