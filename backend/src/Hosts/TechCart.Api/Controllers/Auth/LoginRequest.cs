using System.ComponentModel.DataAnnotations;

namespace TechCart.Api.Controllers.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    public string Password { get; set; } = default!;
}