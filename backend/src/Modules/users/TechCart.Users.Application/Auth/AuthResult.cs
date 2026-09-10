namespace TechCart.Users.Application.Auth;

public record AuthResult(string Token, AuthUserDto User);

public record AuthUserDto(
    Guid Id, 
    string FirstName, 
    string LastName, 
    string Email, 
    string Role);
