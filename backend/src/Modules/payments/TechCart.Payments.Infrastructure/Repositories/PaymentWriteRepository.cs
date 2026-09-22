using Microsoft.EntityFrameworkCore;
using TechCart.Payments.Domain.Entities;
using TechCart.Payments.Domain.Repositories;

namespace TechCart.Payments.Infrastructure.Repositories;

public class PaymentWriteRepository : IPaymentWriteRepository
{
    private readonly PaymentsDbContext _dbContext;

    public PaymentWriteRepository(PaymentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByProviderReferenceAsync(string providerReference, CancellationToken ct)
        => _dbContext.Payments.AsNoTracking().AnyAsync(p => p.ProviderReference == providerReference, ct);

    public async Task AddAsync(Payment payment, CancellationToken ct)
        => await _dbContext.Payments.AddAsync(payment, ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
}