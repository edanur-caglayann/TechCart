using TechCart.SharedKernel.Entities;

namespace TechCart.Brands.Domain.Entities;

public sealed class Brand : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}
