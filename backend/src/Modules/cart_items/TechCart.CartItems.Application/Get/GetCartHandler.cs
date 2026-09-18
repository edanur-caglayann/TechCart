using TechCart.CartItems.Application.Abstractions;
using TechCart.CartItems.Contracts;
using TechCart.CartItems.Contracts.Dtos.ResponseDtos;
using TechCart.Products.Application.Summaries;

namespace TechCart.CartItems.Application.Get;

public record GetCartQuery(Guid UserId);

public class GetCartHandler
{
    private const decimal ShippingFee = 0m; // MVP kapsamında her zaman 0 — 4.7

    private readonly ICartItemReadRepository _cartItemReadRepository;
    private readonly GetProductSummariesHandler _getProductSummariesHandler;

    public GetCartHandler(ICartItemReadRepository cartItemReadRepository, GetProductSummariesHandler getProductSummariesHandler)
    {
        _cartItemReadRepository = cartItemReadRepository;
        _getProductSummariesHandler = getProductSummariesHandler;
    }

    public async Task<CartResponse> Handle(GetCartQuery query, CancellationToken ct)
    {
        var cartItems = await _cartItemReadRepository.GetAllByUserAsync(query.UserId, ct);

        if (cartItems.Count == 0)
            return new CartResponse([], 0, 0, 0, ShippingFee, ShippingFee);

        var productIds = cartItems.Select(c => c.ProductId).ToList();
        var summaries = await _getProductSummariesHandler.Handle(new GetProductSummariesQuery(productIds), ct);
        var summariesById = summaries.ToDictionary(s => s.Id);

        var items = new List<CartItemResponse>();

        foreach (var cartItem in cartItems)
        {
            if (!summariesById.TryGetValue(cartItem.ProductId, out var summary))
                continue;

            // summary.Price zaten KDV dahil. vatAmount, KDV DAHİL lineTotal'ın
            // içinden ters formülle çıkarılıyor — tutar * oran / (1 + oran)
            var unitPrice = summary.Price;
            var lineTotal = Math.Round(unitPrice * cartItem.Quantity, 2);
            var vatAmount = Math.Round(lineTotal * summary.VatRate / (1 + summary.VatRate), 2);

            items.Add(new CartItemResponse(
                summary.Id, summary.Name, summary.Model, summary.Brand, summary.Category, summary.Image,
                unitPrice, summary.VatRate, cartItem.Quantity, lineTotal, vatAmount,
                summary.Stock, summary.Stock > 0));
        }

        var totalQuantity = items.Sum(i => i.Quantity);
        // subtotal + vatTotal, her satırın kendi ve vatAmount toplamından hesaplanıyor 
        var vatTotal = items.Sum(i => i.VatAmount);
        var subtotal = items.Sum(i => i.LineTotal - i.VatAmount);
        var total = subtotal + vatTotal + ShippingFee;

        return new CartResponse(items, totalQuantity, subtotal, vatTotal, ShippingFee, total);
    }
}