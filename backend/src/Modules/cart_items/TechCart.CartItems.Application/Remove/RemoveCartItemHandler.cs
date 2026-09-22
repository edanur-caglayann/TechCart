using TechCart.CartItems.Application.Abstractions;
using TechCart.CartItems.Application.Get;
using TechCart.CartItems.Contracts;
using TechCart.CartItems.Domain.Exceptions;
using TechCart.CartItems.Domain.Repositories;

namespace TechCart.CartItems.Application.Remove;

public record RemoveCartItemCommand(Guid UserId, Guid ProductId);

public class RemoveCartItemHandler(ICartItemWriteRepository cartItemWriteRepository, GetCartHandler getCartHandler)
{
    public async Task<CartResponse> Handle(RemoveCartItemCommand command, CancellationToken ct)
    {
        var cartItem = await cartItemWriteRepository.GetByUserAndProductAsync(command.UserId, command.ProductId, ct)
                       ?? throw new CartItemNotFoundException(command.ProductId);

        cartItemWriteRepository.Remove(cartItem);
        await cartItemWriteRepository.SaveChangesAsync(ct);

        return await getCartHandler.Handle(new GetCartQuery(command.UserId), ct);
    }
}