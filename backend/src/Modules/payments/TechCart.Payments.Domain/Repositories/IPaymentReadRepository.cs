using TechCart.Payments.Contracts.ResponseDtos;

namespace TechCart.Payments.Domain.Repositories;

public interface IPaymentReadRepository
{
    Task<List<PaymentSummaryDto>> GetSucceededByOrderIdsAsync(List<Guid> orderIds, CancellationToken ct);
}