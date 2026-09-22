using Microsoft.EntityFrameworkCore;
using TechCart.CartItems.Domain.Entities;
using TechCart.CartItems.Domain.Repositories;

namespace TechCart.CartItems.Infrastructure.Repositories;

public class CartItemWriteRepository : ICartItemWriteRepository
{
    private readonly CartItemsDbContext _dbContext;

    public CartItemWriteRepository(CartItemsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CartItem?> GetByUserAndProductAsync(Guid userId, Guid productId, CancellationToken ct)
        => _dbContext.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId, ct);

    public Task<List<CartItem>> GetAllByUserAsync(Guid userId, CancellationToken ct)
        => _dbContext.CartItems.Where(c => c.UserId == userId).ToListAsync(ct);

    public async Task AddAsync(CartItem cartItem, CancellationToken ct)
        => await _dbContext.CartItems.AddAsync(cartItem, ct);

    public void Remove(CartItem cartItem)
        => _dbContext.CartItems.Remove(cartItem);

    public void RemoveRange(IEnumerable<CartItem> cartItems)
        => _dbContext.CartItems.RemoveRange(cartItems);

    public Task SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
}