using TechCart.SharedKernel;
using TechCart.SharedKernel.Entities;

namespace TechCart.ProductImages.Domain.Entities;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ImageUrl { get; private set; } = default!;
    public int SortOrder { get; private set; }

    private ProductImage() { } 

    private ProductImage(Guid productId, string imageUrl, int sortOrder)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ImageUrl = imageUrl;
        SortOrder = sortOrder;
    }

    public static ProductImage Create(Guid productId, string imageUrl, int sortOrder)
        => new(productId, imageUrl, sortOrder);
}