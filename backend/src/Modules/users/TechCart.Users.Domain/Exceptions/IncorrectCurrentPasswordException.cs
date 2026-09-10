using TechCart.SharedKernel;

namespace TechCart.Users.Domain.Exceptions;

// (enumeration koruması). Burada kullanıcı kimliği doğrulanmış (JWT ile giriş yapmış),
// sadece "mevcut şifreni doğru yazmadın" hatasi  
public class IncorrectCurrentPasswordException : AppException
{
    public IncorrectCurrentPasswordException()
        : base("INCORRECT_CURRENT_PASSWORD", "Mevcut şifreniz hatalı.", statusCode: 400) { }
}