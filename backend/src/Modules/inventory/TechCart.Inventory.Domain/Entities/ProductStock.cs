namespace TechCart.Inventory.Domain.Entities;

// diagramdaki gibi 1-1 ilişki: her ürünü bir stok kaydı olabilir
public class ProductStock
{
    public Guid ProductId { get; private set; }
    public int Stock { get; private set; }
    public bool IsReadyToShip { get; private set; }
    public bool HasFastDelivery { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ProductStock() { } 

    private ProductStock(Guid productId, int stock, bool isReadyToShip, bool hasFastDelivery)
    {
        ProductId = productId;
        Stock = stock;
        IsReadyToShip = isReadyToShip;
        HasFastDelivery = hasFastDelivery;
        UpdatedAt = DateTime.UtcNow;
    }

    public static ProductStock Create(Guid productId, int stock, bool isReadyToShip, bool hasFastDelivery)
        => new(productId, stock, isReadyToShip, hasFastDelivery);
}