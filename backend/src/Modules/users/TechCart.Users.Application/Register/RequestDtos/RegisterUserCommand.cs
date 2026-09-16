namespace TechCart.Users.Application.Register.RequestDtos;

// frontend/API tarafindan gonderilen kayit bilgilerini tasir
public record RegisterUserCommand(string FirstName, string LastName, string Email, string Password);
