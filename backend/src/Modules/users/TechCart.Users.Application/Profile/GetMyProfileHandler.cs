using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Profile.RequestDtos;
using TechCart.Users.Application.Profile.ResponseDtos;
using TechCart.Users.Domain.Exceptions;

namespace TechCart.Users.Application.Profile;

public class GetMyProfileHandler
{
    private readonly IUserReadRepository _userReadRepository;
    public GetMyProfileHandler(IUserReadRepository userReadRepository) => _userReadRepository = userReadRepository;

    public async Task<UserProfileResponse> Handle(GetMyProfileQuery query, CancellationToken ct)
    {
        var profile = await _userReadRepository.GetProfileByIdAsync(query.UserId, ct);
        return profile ?? throw new UserNotFoundException(query.UserId);
    }
}
