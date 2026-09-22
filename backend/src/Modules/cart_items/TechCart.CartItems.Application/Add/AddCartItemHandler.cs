
using TechCart.CartItems.Application.Get;
using TechCart.CartItems.Contracts;
using TechCart.CartItems.Domain.Entities;
using TechCart.CartItems.Domain.Exceptions;
using TechCart.CartItems.Domain.Repositories;
using TechCart.Products.Application.Summaries;

namespace TechCart.CartItems.Application.Add;

public record AddCartItemCommand(Guid UserId, Guid ProductId, int Quantity);

public class AddCartItemHandler(
    ICartItemWriteRepository cartItemWriteRepository,
    GetProductSummariesHandler getProductSummariesHandler,
    GetCartHandler getCartHandler)
{
    private readonly GetCartHandler _getCartHandler = getCartHandler;

    public async Task<CartResponse> Handle(AddCartItemCommand command, CancellationToken ct)
    {
        // stok miktariyla birlikte urun bilgilerini alir
        var summaries = await getProductSummariesHandler.Handle(
            new GetProductSummariesQuery([command.ProductId]), ct);
        var product = summaries.FirstOrDefault()
            ?? throw new ProductNotFoundException(command.ProductId);

        // bu kullanicnin sepetine boyle bir urunu var mi kontorlu
        var existingItem = await cartItemWriteRepository.GetByUserAndProductAsync(command.UserId, command.ProductId, ct);

        // sepette yoksa yeni sepet kaydi olusturur
        if (existingItem is null)
        {
            if (command.Quantity > product.Stock)
                throw new InsufficientStockException(command.ProductId, product.Stock);

            var cartItem = CartItem.Create(command.UserId, command.ProductId, command.Quantity);
            await cartItemWriteRepository.AddAsync(cartItem, ct);
        }
        else
        {
            // urun adedidini toplayarak ilerler
            var newQuantity = existingItem.Quantity + command.Quantity;
            if (newQuantity > product.Stock)
                throw new InsufficientStockException(command.ProductId, product.Stock);

            existingItem.IncreaseQuantity(command.Quantity);
        }

        await cartItemWriteRepository.SaveChangesAsync(ct);

        return await _getCartHandler.Handle(new GetCartQuery(command.UserId), ct);
    }
}