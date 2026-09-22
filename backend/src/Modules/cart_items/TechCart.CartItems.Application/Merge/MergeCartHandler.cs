using TechCart.CartItems.Application.Abstractions;
using TechCart.CartItems.Application.Get;
using TechCart.CartItems.Contracts;
using TechCart.CartItems.Domain.Entities;
using TechCart.CartItems.Domain.Repositories;
using TechCart.Products.Application.Summaries;

namespace TechCart.CartItems.Application.Merge;

// localStorage'den gelen misafir sepetindeki urun id + adedi
public record MergeCartItem(Guid ProductId, int Quantity);

// sepeti birlestirilecek kullanici ve misafir sepetindeki urunler
public record MergeCartCommand(Guid UserId, List<MergeCartItem> Items);

public class MergeCartHandler(
    ICartItemWriteRepository cartItemWriteRepository,
    GetProductSummariesHandler getProductSummariesHandler,
    GetCartHandler getCartHandler)
{
    public async Task<CartResponse> Handle(MergeCartCommand command, CancellationToken ct)
    {
        // misafir hic urun eklememisse mevcut sepeti dondur
        if (command.Items.Count == 0)
            return await getCartHandler.Handle(new GetCartQuery(command.UserId), ct);
        
        // urun bilgilerini topluca getirir
        var productIds = command.Items.Select(i => i.ProductId).Distinct().ToList();
        var summaries = await getProductSummariesHandler.Handle(new GetProductSummariesQuery(productIds), ct);
        var summariesById = summaries.ToDictionary(s => s.Id);

        // her urun icin: yoksa olustur, varsa topla, stokla sinirla 
        foreach (var incomingItem in command.Items)
        {
            // urun bilgisi bulunamazsa o urunu atlar
            if (!summariesById.TryGetValue(incomingItem.ProductId, out var product))
                continue;
            
            // bu urun bu kullanicinin sepetinde var mi
            var existingItem = await cartItemWriteRepository.GetByUserAndProductAsync(command.UserId, incomingItem.ProductId, ct);

            // urun sepette yoksa kayit olusturulur
            if (existingItem is null)
            {
                var quantity = Math.Min(incomingItem.Quantity, product.Stock);
                if (quantity < 1) continue;
                
                // yeni sepet kaydi olusturulur 
                var cartItem = CartItem.Create(command.UserId, incomingItem.ProductId, quantity);
                await cartItemWriteRepository.AddAsync(cartItem, ct);
            }
            
            // urun mevcut sepette varsa adetleri toplanir
            else
            {
                var desiredQuantity = existingItem.Quantity + incomingItem.Quantity;
                var cappedQuantity = Math.Min(desiredQuantity, product.Stock);
                var amountToIncrease = cappedQuantity - existingItem.Quantity;

                if (amountToIncrease > 0)
                    existingItem.IncreaseQuantity(amountToIncrease);
            }
        }

        await cartItemWriteRepository.SaveChangesAsync(ct);

        return await getCartHandler.Handle(new GetCartQuery(command.UserId), ct);
    }
}