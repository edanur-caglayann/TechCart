using TechCart.CartItems.Application.Abstractions;
using TechCart.CartItems.Contracts;
using TechCart.CartItems.Domain.Repositories;

namespace TechCart.CartItems.Application.Clear;

public record ClearCartCommand(Guid UserId);

public class ClearCartHandler(ICartItemWriteRepository cartItemWriteRepository)
{
    public async Task<CartResponse> Handle(ClearCartCommand command, CancellationToken ct)
    {
        var cartItems = await cartItemWriteRepository.GetAllByUserAsync(command.UserId, ct);

        cartItemWriteRepository.RemoveRange(cartItems);
        await cartItemWriteRepository.SaveChangesAsync(ct);

        // Sepet zaten boşaldığı için GetCartHandler'a hiç gitmeye gerek yok 
        return new CartResponse([], 0, 0, 0, 0, 0);    }
}