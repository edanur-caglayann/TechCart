using System.ComponentModel.DataAnnotations;

namespace TechCart.Api.Controllers.Users;

public class UpdateProfileRequest
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
}