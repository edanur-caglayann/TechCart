namespace TechCart.Categories.Application.Abstractions;

public record CategoryDto(Guid Id, string Name);

public interface ICategoryReadRepository
{
    Task<List<CategoryDto>> GetAllAsync(CancellationToken ct);
}