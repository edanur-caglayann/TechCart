using TechCart.Payments.Application.Dtos.RequestDtos;
using TechCart.Payments.Contracts.ResponseDtos;
using TechCart.Payments.Domain.Repositories;

namespace TechCart.Payments.Application.List;

public class ListPaymentSummariesHandler(IPaymentReadRepository paymentReadRepository)
{
    public Task<List<PaymentSummaryDto>> Handle(ListPaymentSummariesQuery query, CancellationToken ct)
        => paymentReadRepository.GetSucceededByOrderIdsAsync(query.OrderIds, ct);
}