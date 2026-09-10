using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Auth;
using TechCart.Users.Domain.Entities;
using TechCart.Users.Domain.Exceptions;
using TechCart.Users.Domain.Repositories;

namespace TechCart.Users.Application.Register;

// frontend/API tarafindan gonderilen kayit bilgilerini tasir
public record RegisterUserCommand(string FirstName, string LastName, string Email, string Password);

public class RegisterUserHandler
{
    private readonly IUserWriteRepository _userWriteRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator; 
    
    public RegisterUserHandler(IUserWriteRepository userWriteRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator)
    {
        _userWriteRepository = userWriteRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    // donus tipi AuthResult: kullanici bilgileri ve token 
    public async Task<AuthResult> Handle(RegisterUserCommand command, CancellationToken ct) 
    {
        var alreadyExists = await _userWriteRepository.ExistsByEmailAsync(command.Email, ct);
        if (alreadyExists)
            throw new EmailAlreadyExistsException(command.Email); // InvalidOperationException -> özel exception

        var passwordHash = _passwordHasher.Hash(command.Password);
        var user = User.Register(command.FirstName, command.LastName, command.Email, passwordHash);

        await _userWriteRepository.AddAsync(user, ct);
        await _userWriteRepository.SaveChangesAsync(ct);

        // kayıt sonrası hemen token üretir, kullanıcı tekrar giriş yapmak zorunda kalmaz.
        var token = _tokenGenerator.GenerateToken(user.Id, user.FirstName, user.LastName, user.Email, user.Role);
        // frontend'e token ve kullanici bilgileri gonderilir.
        return new AuthResult(token, new AuthUserDto(user.Id, user.FirstName, user.LastName, user.Email, user.Role.ToString()));
    }
}