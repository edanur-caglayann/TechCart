using TechCart.CartItems.Application.Dtos.ResponseDtos;

namespace TechCart.CartItems.Application.Abstractions;

public interface ICartItemReadRepository
{
    Task<List<CartItemRowDto>> GetAllByUserAsync(Guid userId, CancellationToken ct);
}
