namespace TechCart.Users.Application.Profile.RequestDtos;

public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword);
