using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Auth.ResponseDtos;
using TechCart.Users.Application.Register.RequestDtos;
using TechCart.Users.Domain.Entities;
using TechCart.Users.Domain.Exceptions;
using TechCart.Users.Domain.Repositories;

namespace TechCart.Users.Application.Register;

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

    // donus tipi AuthResponse: kullanici bilgileri ve token
    public async Task<AuthResponse> Handle(RegisterUserCommand command, CancellationToken ct)
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
        // Yanitta token ve kullanici bilgileri donulur.
        return new AuthResponse(token, new AuthUserResponse(user.Id, user.FirstName, user.LastName, user.Email, user.Role.ToString()));
    }
}
