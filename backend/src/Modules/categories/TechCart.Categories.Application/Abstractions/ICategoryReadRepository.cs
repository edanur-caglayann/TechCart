using TechCart.Categories.Application.Dtos.ResponseDtos;

namespace TechCart.Categories.Application.Abstractions;

public interface ICategoryReadRepository
{
    Task<List<CategoryDto>> GetAllAsync(CancellationToken ct);
    Task<List<CategoryDto>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);
    Task<List<string>> SearchByNameAsync(string term, int limit, CancellationToken ct);
}
