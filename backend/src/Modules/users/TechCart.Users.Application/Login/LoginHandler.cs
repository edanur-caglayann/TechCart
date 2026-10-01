using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Dtos.ResponseDtos;
using TechCart.Users.Application.Dtos.RequestDtos;
using TechCart.Users.Domain.Exceptions;

namespace TechCart.Users.Application.Login;

public class LoginHandler(
    IUserReadRepository userReadRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator)
{
    public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken ct)
    {
        var credentials = await userReadRepository.GetCredentialsByEmailAsync(command.Email, ct);

        // credentials null (e-posta yok) veya hash uyuşmuyor (şifre yanlış) 
        if (credentials is null || !passwordHasher.Verify(command.Password, credentials.PasswordHash))
            throw new InvalidCredentialsException();

        var token = tokenGenerator.GenerateToken(credentials.Id, credentials.FirstName, credentials.LastName, credentials.Email, credentials.Role);
        return new AuthResponse(token, new AuthUserResponse(credentials.Id, credentials.FirstName, credentials.LastName, credentials.Email, credentials.Role.ToString()));
    }
}
