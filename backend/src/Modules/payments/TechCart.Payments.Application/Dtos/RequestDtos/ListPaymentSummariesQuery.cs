namespace TechCart.Payments.Application.Dtos.RequestDtos;

public record ListPaymentSummariesQuery(List<Guid> OrderIds);