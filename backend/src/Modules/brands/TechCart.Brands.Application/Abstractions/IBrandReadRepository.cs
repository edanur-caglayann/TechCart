namespace TechCart.Brands.Application.Abstractions;

public record BrandDto(Guid Id, string Name);

public interface IBrandReadRepository
{
    Task<List<BrandDto>> GetAllAsync(CancellationToken ct);
}