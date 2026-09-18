using TechCart.CartItems.Application.Abstractions;
using TechCart.CartItems.Application.Get;
using TechCart.CartItems.Contracts;
using TechCart.CartItems.Domain.Exceptions;
using TechCart.CartItems.Domain.Repositories;
using TechCart.Products.Application.Summaries;

namespace TechCart.CartItems.Application.Update;

public record UpdateCartItemQuantityCommand(Guid UserId, Guid ProductId, int Quantity);

public class UpdateCartItemQuantityHandler(
    ICartItemWriteRepository cartItemWriteRepository,
    GetProductSummariesHandler getProductSummariesHandler,
    GetCartHandler getCartHandler)
{
    public async Task<CartResponse> Handle(UpdateCartItemQuantityCommand command, CancellationToken ct)
    {
        if (command.Quantity < 1)
            throw new InvalidQuantityException();

        var cartItem = await cartItemWriteRepository.GetByUserAndProductAsync(command.UserId, command.ProductId, ct)
                       ?? throw new CartItemNotFoundException(command.ProductId);

        var summaries = await getProductSummariesHandler.Handle(
            new GetProductSummariesQuery([command.ProductId]), ct);
        var product = summaries.First(); // sepette olduğuna göre Products'ta da vardır

        if (command.Quantity > product.Stock)
            throw new InsufficientStockException(command.ProductId, product.Stock);

        cartItem.SetQuantity(command.Quantity);
        await cartItemWriteRepository.SaveChangesAsync(ct);

        return await getCartHandler.Handle(new GetCartQuery(command.UserId), ct);
    }
}