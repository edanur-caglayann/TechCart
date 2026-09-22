namespace TechCart.Users.Application.Dtos.RequestDtos;

public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword);
