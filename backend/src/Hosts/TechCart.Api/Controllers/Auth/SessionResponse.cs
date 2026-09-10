using TechCart.Users.Application.Auth;

namespace TechCart.Api.Controllers.Auth;

public record SessionResponse(bool IsAuthenticated, AuthUserDto User);