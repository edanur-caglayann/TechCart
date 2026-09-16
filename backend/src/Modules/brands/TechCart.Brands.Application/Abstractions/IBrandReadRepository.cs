namespace TechCart.Brands.Application.Abstractions;

public record BrandDto(Guid Id, string Name);

public interface IBrandReadRepository
{
    Task<List<BrandDto>> GetAllAsync(CancellationToken ct);
    Task<List<BrandDto>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);

    Task<List<string>> SearchByNameAsync(string term, int limit, CancellationToken ct);
}