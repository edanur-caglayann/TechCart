using TechCart.Users.Application.Abstractions;
using TechCart.Users.Domain.Exceptions;
using TechCart.Users.Domain.Repositories;

namespace TechCart.Users.Application.Profile;

public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword);

public class ChangePasswordHandler
{
    private readonly IUserWriteRepository _userWriteRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordHandler(IUserWriteRepository userWriteRepository, IPasswordHasher passwordHasher)
    {
        _userWriteRepository = userWriteRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ChangePasswordCommand command, CancellationToken ct)
    {
        var user = await _userWriteRepository.GetByIdAsync(command.UserId, ct)
                   ?? throw new UserNotFoundException(command.UserId);

        // Mevcut şifre db'deki ile ayni mi kontrolü 
        if (!_passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
            throw new IncorrectCurrentPasswordException();

        var newHash = _passwordHasher.Hash(command.NewPassword);
        user.ChangePassword(newHash);
        await _userWriteRepository.SaveChangesAsync(ct);
    }
}