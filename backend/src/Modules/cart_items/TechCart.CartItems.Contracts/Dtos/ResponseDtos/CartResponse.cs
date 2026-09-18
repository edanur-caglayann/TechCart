using TechCart.CartItems.Contracts.Dtos.ResponseDtos;

namespace TechCart.CartItems.Contracts;

// sepetin tamami
public record CartResponse(
    List<CartItemResponse> Items,
    int TotalQuantity,
    decimal Subtotal, // KDV hariç toplam
    decimal VatTotal, //  toplam KDV
    decimal ShippingFee, // MVP'de her zaman 0
    decimal Total); 