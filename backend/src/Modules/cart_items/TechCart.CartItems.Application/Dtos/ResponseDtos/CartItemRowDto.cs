namespace TechCart.CartItems.Application.Dtos.ResponseDtos;

// sadece urun id + adet. sepette hicbir zaman fiyat saklanmaz. 
// GetCartHandler ile product'tan guncel fiyati cekip birlestirirz
public record CartItemRowDto(Guid ProductId, int Quantity);
