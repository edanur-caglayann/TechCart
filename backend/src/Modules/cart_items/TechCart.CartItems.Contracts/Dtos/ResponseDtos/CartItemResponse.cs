namespace TechCart.CartItems.Contracts.Dtos.ResponseDtos;

// sepetteki tek bir urun icin donen degerler
public record CartItemResponse(
    Guid ProductId,
    string Name,
    string Model,
    string Brand,
    string Category,
    string? Image,
    decimal UnitPrice,// KDV dahil birim fiyat
    decimal VatRate,
    int Quantity,
    decimal LineTotal,// UnitPrice * Quantity
    decimal VatAmount, // LineTotal'ın içindeki KDV payı
    int Stock,
    bool InStock);