using TechCart.Brands.Application.Abstractions;

namespace TechCart.Brands.Application.List;

public class ListBrandsHandler
{
    private readonly IBrandReadRepository _brandReadRepository;
    public ListBrandsHandler(IBrandReadRepository brandReadRepository) => _brandReadRepository = brandReadRepository;

    public Task<List<BrandDto>> Handle(CancellationToken ct)
        => _brandReadRepository.GetAllAsync(ct);
}