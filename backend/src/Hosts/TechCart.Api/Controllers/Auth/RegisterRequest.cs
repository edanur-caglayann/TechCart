using System.ComponentModel.DataAnnotations;

namespace TechCart.Api.Controllers.Auth;

// kullanican kayit bigileri alinir, dogrulanir, parola hash'lenir
// kullanici db kaydedilir ve kullaniciya jwt uretilir.
public class RegisterRequest
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [MaxLength(100)]
    public string FirstName { get; set; } = default!;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [MaxLength(100)]
    public string LastName { get; set; } = default!;

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [MaxLength(256)]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalıdır.")]
    [MaxLength(100)]
    public string Password { get; set; } = default!;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
    public string PasswordConfirm { get; set; } = default!;
}