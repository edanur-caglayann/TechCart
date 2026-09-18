namespace TechCart.Users.Application.Register.RequestDtos;

// API tarafindan alinan kayit bilgilerini tasir
public record RegisterUserCommand(string FirstName, string LastName, string Email, string Password);
