using TechCart.Brands.Application.Abstractions;
using TechCart.Brands.Application.Dtos.ResponseDtos;

namespace TechCart.Brands.Application.List;

public class ListBrandsHandler
{
    private readonly IBrandReadRepository _brandReadRepository;
    public ListBrandsHandler(IBrandReadRepository brandReadRepository) => _brandReadRepository = brandReadRepository;

    public Task<List<BrandResponse>> Handle(CancellationToken ct)
        => _brandReadRepository.GetAllAsync(ct);
}
