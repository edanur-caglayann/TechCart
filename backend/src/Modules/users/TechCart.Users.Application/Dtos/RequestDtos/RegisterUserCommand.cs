namespace TechCart.Users.Application.Dtos.RequestDtos;

// API tarafindan alinan kayit bilgilerini tasir
public record RegisterUserCommand(string FirstName, string LastName, string Email, string Password);
