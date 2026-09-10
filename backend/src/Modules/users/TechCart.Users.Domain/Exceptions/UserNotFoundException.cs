using TechCart.SharedKernel;

namespace TechCart.Users.Domain.Exceptions;

// Token geçerliyse kullanıcı normalde DB'de olmalı; yine de kullanıcı silinmiş mi diye kontrol
public class UserNotFoundException : AppException
{
    public UserNotFoundException(Guid userId)
        : base("USER_NOT_FOUND", $"'{userId}' id'li kullanıcı bulunamadı.", statusCode: 404) { }
}