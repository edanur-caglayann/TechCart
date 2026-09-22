namespace TechCart.Payments.Application.Dtos.ResponseDtos;

public record PaymentResultDto(
    bool IsSuccessful,
    string ConversationId,   // Initialize'da bizim gönderdiğimiz kimlik — bununla hangi Order olduğunu buluyoruz
    string ProviderPaymentId, // iyzico'nun KENDİ işlem kimliği — ProviderReference/UNIQUE kısıt için
    decimal PaidPrice,
    string? MaskedCardNumber,
    string? FailureReason);
    
    // ConversationId -> hangi siparis
    // ProviderPaymentId -> hangi odeme denemesi 