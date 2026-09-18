using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Dtos.RequestDtos;
using TechCart.Users.Application.Dtos.ResponseDtos;
using TechCart.Users.Domain.Exceptions;
using TechCart.Users.Domain.Repositories;

namespace TechCart.Users.Application.Profile;

public class UpdateProfileHandler
{
    private readonly IUserWriteRepository _userWriteRepository;
    public UpdateProfileHandler(IUserWriteRepository userWriteRepository) => _userWriteRepository = userWriteRepository;

    public async Task<UserProfileResponse> Handle(UpdateProfileCommand command, CancellationToken ct)
    {
        var user = await _userWriteRepository.GetByIdAsync(command.UserId, ct)
                   ?? throw new UserNotFoundException(command.UserId);

        // ayni e postayi girdiginde kendi kaydına çarpıp "zaten kayıtlı" hatası almasın diye excludeUserId veriyoruz.
        var emailTakenByAnother = await _userWriteRepository.ExistsByEmailAsync(command.Email, ct, excludeUserId: command.UserId);
        if (emailTakenByAnother)
            throw new EmailAlreadyExistsException(command.Email);

        user.UpdateProfile(command.FirstName, command.LastName, command.Email);
        await _userWriteRepository.SaveChangesAsync(ct);

        return new UserProfileResponse(user.Id, user.FirstName, user.LastName, user.Email, user.Role.ToString(), user.CreatedAt);
    }
}
