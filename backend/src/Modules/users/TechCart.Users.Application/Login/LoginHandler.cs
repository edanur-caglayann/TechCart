using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Dtos.ResponseDtos;
using TechCart.Users.Application.Dtos.RequestDtos;
using TechCart.Users.Domain.Exceptions;

namespace TechCart.Users.Application.Login;

public class LoginHandler
{
    private readonly IUserReadRepository _userReadRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginHandler(IUserReadRepository userReadRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator)
    {
        _userReadRepository = userReadRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken ct)
    {
        var credentials = await _userReadRepository.GetCredentialsByEmailAsync(command.Email, ct);

        // credentials null (e-posta yok) veya hash uyuşmuyor (şifre yanlış) 
        if (credentials is null || !_passwordHasher.Verify(command.Password, credentials.PasswordHash))
            throw new InvalidCredentialsException();

        var token = _tokenGenerator.GenerateToken(credentials.Id, credentials.FirstName, credentials.LastName, credentials.Email, credentials.Role);
        return new AuthResponse(token, new AuthUserResponse(credentials.Id, credentials.FirstName, credentials.LastName, credentials.Email, credentials.Role.ToString()));
    }
}
