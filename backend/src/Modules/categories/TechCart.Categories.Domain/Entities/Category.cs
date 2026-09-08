using TechCart.SharedKernel.Entities;

namespace TechCart.Categories.Domain.Entities;

public sealed class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}
