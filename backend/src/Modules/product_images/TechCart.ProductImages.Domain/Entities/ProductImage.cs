using TechCart.SharedKernel.Entities;

namespace TechCart.ProductImages.Domain.Entities;

public sealed class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
