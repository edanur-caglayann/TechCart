using TechCart.Categories.Application.Abstractions;

namespace TechCart.Categories.Application.List;

// tum kategorileri listeliyor. Diger handler'lar gibi Query / Commond nesnesi almiyor.
// cunku bir girdiye ihtiyaci yok
public class ListCategoriesHandler
{
    private readonly ICategoryReadRepository _categoryReadRepository;
    public ListCategoriesHandler(ICategoryReadRepository categoryReadRepository) => _categoryReadRepository = categoryReadRepository;

    public Task<List<CategoryDto>> Handle(CancellationToken ct)
        => _categoryReadRepository.GetAllAsync(ct);
}