using System.ComponentModel.DataAnnotations;

namespace TechCart.Api.Controllers.Users;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Mevcut şifre zorunludur.")]
    public string CurrentPassword { get; set; } = default!;

    [Required(ErrorMessage = "Yeni şifre zorunludur.")]
    [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalıdır.")]
    [MaxLength(100)]
    public string NewPassword { get; set; } = default!;

    [Required(ErrorMessage = "Yeni şifre tekrarı zorunludur.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Şifreler eşleşmiyor.")]
    public string NewPasswordConfirm { get; set; } = default!;
}