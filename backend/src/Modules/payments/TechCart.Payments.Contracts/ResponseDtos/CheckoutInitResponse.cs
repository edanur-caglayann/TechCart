namespace TechCart.Payments.Contracts.ResponseDtos;

// PaymentPageUrl -> iyzico'nun hazirladigi kullanciinin kart bilgilerini girecegei sayfanin adresi
// odeme denemesi sonucu iyzico'nun verdigi kimlik numarasi
public record CheckoutInitResponse(string PaymentPageUrl, string Token);