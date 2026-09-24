using Microsoft.EntityFrameworkCore;
using TechCart.Payments.Contracts.ResponseDtos;
using TechCart.Payments.Domain.Enums;
using TechCart.Payments.Domain.Repositories;

namespace TechCart.Payments.Infrastructure.Repositories;

public class PaymentReadRepository(PaymentsDbContext dbContext) : IPaymentReadRepository
{
    public Task<List<PaymentSummaryDto>> GetSucceededByOrderIdsAsync(List<Guid> orderIds, CancellationToken ct)
        => dbContext.Payments.AsNoTracking()
            .Where(p => orderIds.Contains(p.OrderId) && p.Status == PaymentStatus.Succeeded)
            .Select(p => new PaymentSummaryDto(p.OrderId, p.Provider, p.MaskedCardNumber, p.Amount, p.CreatedAt))
            .ToListAsync(ct);
}