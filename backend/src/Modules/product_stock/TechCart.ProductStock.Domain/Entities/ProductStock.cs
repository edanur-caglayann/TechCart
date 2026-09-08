namespace TechCart.ProductStock.Domain.Entities;

public sealed class ProductStock
{
    public Guid ProductId { get; set; }

    public int Stock { get; set; }

    public bool IsReadyToShip { get; set; }

    public bool HasFastDelivery { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
