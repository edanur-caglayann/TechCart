using System.ComponentModel.DataAnnotations;

namespace TechCart.Api.Controllers.Addresses;

public class AddressRequest
{
    [Required(ErrorMessage = "Başlık zorunludur.")]
    [MaxLength(100)]
    public string Title { get; set; } = default!;

    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [MaxLength(200)]
    public string FullName { get; set; } = default!;

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [MaxLength(20)]
    public string Phone { get; set; } = default!;

    [Required(ErrorMessage = "Şehir zorunludur.")]
    [MaxLength(100)]
    public string City { get; set; } = default!;

    [Required(ErrorMessage = "İlçe zorunludur.")]
    [MaxLength(100)]
    public string District { get; set; } = default!;
    
    [Required(ErrorMessage = "Mahalle zorunludur.")]
    [MaxLength(150)]
    
    public string Neighborhood { get; set; } = default!;
    [Required(ErrorMessage = "Adres satırı zorunludur.")]
    
    public string AddressLine { get; set; } = default!;

    [Required(ErrorMessage = "Posta kodu zorunludur.")]
    [MaxLength(20)]
    public string PostalCode { get; set; } = default!;
}