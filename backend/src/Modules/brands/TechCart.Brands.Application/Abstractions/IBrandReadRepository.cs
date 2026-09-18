using TechCart.Brands.Application.Dtos.ResponseDtos;

namespace TechCart.Brands.Application.Abstractions;

public interface IBrandReadRepository
{
    Task<List<BrandResponse>> GetAllAsync(CancellationToken ct);
    Task<List<BrandResponse>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);

    Task<List<string>> SearchByNameAsync(string term, int limit, CancellationToken ct);
}
