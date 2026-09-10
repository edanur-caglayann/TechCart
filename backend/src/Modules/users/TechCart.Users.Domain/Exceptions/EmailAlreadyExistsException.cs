using TechCart.SharedKernel;

namespace TechCart.Users.Domain.Exceptions;


public class EmailAlreadyExistsException : AppException
{
    public EmailAlreadyExistsException(string email)
        : base("EMAIL_ALREADY_EXISTS", $"'{email}' e-posta adresi zaten kayıtlı.", statusCode: 409) { }
}