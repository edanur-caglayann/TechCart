using Microsoft.EntityFrameworkCore;
using TechCart.CartItems.Application.Abstractions;
using TechCart.CartItems.Application.Dtos.ResponseDtos;

namespace TechCart.CartItems.Infrastructure.Repositories;

public class CartItemReadRepository : ICartItemReadRepository
{
    private readonly CartItemsDbContext _dbContext;

    public CartItemReadRepository(CartItemsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<CartItemRowDto>> GetAllByUserAsync(Guid userId, CancellationToken ct)
        => _dbContext.CartItems.AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => new CartItemRowDto(c.ProductId, c.Quantity))
            .ToListAsync(ct);
}
