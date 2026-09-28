import { ApiError } from "../services/apiClient";

/*
  Backend'in AppException kodlarını, kullanıcıya gösterilebilir sade
  Türkçe mesajlara çeviriyoruz. Ham backend mesajları (GUID içeren,
  teknik metinler) hiçbir zaman doğrudan ekrana basılmıyor.
*/
export function getFriendlyErrorMessage(
  error: unknown,
  fallback = "Bir şeyler ters gitti. Lütfen tekrar deneyin."
): string {
  if (error instanceof ApiError) {
    switch (error.code) {
      case "INSUFFICIENT_STOCK":
        return "Üzgünüz, bu üründen yeterli stok kalmadı.";
      case "EMPTY_CART":
        return "Sepetin boş görünüyor.";
      case "ORDER_NOT_AWAITING_PAYMENT":
        return "Bu sipariş için ödeme zaten tamamlanmış veya iptal edilmiş.";
      default:
        return fallback;
    }
  }

  return fallback;
}