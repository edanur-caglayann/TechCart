using TechCart.Users.Application.Abstractions;
using TechCart.Users.Domain.Exceptions;

namespace TechCart.Users.Application.Profile;

public record GetMyProfileQuery(Guid UserId);

public class GetMyProfileHandler
{
    private readonly IUserReadRepository _userReadRepository;
    public GetMyProfileHandler(IUserReadRepository userReadRepository) => _userReadRepository = userReadRepository;

    public async Task<UserProfileDto> Handle(GetMyProfileQuery query, CancellationToken ct)
    {
        var profile = await _userReadRepository.GetProfileByIdAsync(query.UserId, ct);
        return profile ?? throw new UserNotFoundException(query.UserId);
    }
}