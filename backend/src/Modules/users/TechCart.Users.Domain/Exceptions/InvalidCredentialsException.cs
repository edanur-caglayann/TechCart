using TechCart.SharedKernel;

namespace TechCart.Users.Domain.Exceptions;

public class InvalidCredentialsException : AppException
{
    public InvalidCredentialsException()
        : base("INVALID_CREDENTIALS", "E-posta veya şifre hatalı.", statusCode: 401) { }
}