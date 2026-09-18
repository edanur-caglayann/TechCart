using TechCart.Users.Application.Abstractions;
using TechCart.Users.Application.Dtos.RequestDtos;
using TechCart.Users.Application.Dtos.ResponseDtos;
using TechCart.Users.Domain.Exceptions;

namespace TechCart.Users.Application.Auth;

public class GetSessionHandler
{
    private readonly IUserReadRepository _userReadRepository;

    public GetSessionHandler(IUserReadRepository userReadRepository)
        => _userReadRepository = userReadRepository;

    public async Task<SessionResponse> Handle(GetSessionQuery query, CancellationToken ct)
    {
        var profile = await _userReadRepository.GetProfileByIdAsync(query.UserId, ct)
            ?? throw new UserNotFoundException(query.UserId);

        return new SessionResponse(true,
            new AuthUserResponse(profile.Id, profile.FirstName, profile.LastName, profile.Email, profile.Role));
    }
}
