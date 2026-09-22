namespace TechCart.Inventory.Domain.Entities;

// diagramdaki gibi 1-1 ilişki: her ürünü bir stok kaydı olabilir
public class ProductStock
{
    public Guid ProductId { get; private set; }
    public int Stock { get; private set; }
    public int ReservedStock { get; private set; } 
    public bool IsReadyToShip { get; private set; }
    public bool HasFastDelivery { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    // kullaniciya gosterilen "bu urunden kac tane alabilirsin" sayisi
    public int AvailableStock => Stock - ReservedStock;
    
    private ProductStock() { } 

    private ProductStock(Guid productId, int stock, bool isReadyToShip, bool hasFastDelivery)
    {
        ProductId = productId;
        Stock = stock;
        ReservedStock = 0;
        IsReadyToShip = isReadyToShip;
        HasFastDelivery = hasFastDelivery;
        UpdatedAt = DateTime.UtcNow;
    }

    public static ProductStock Create(Guid productId, int stock, bool isReadyToShip, bool hasFastDelivery)
        => new(productId, stock, isReadyToShip, hasFastDelivery);
    
    // SQL update'i ile rezervasyon yapilacak. Bu metot ile de o SQL'in uyguladigi is kuralinin
    // domain'deki karsiligi olacak
    public void Reserve(int quantity)
    {
        if (quantity > AvailableStock)
            throw new InvalidOperationException("Yeterli stok yok.");

        ReservedStock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }
    
    // odeme basarili. rezervasyon kalici duser
    public void ConfirmReservation(int quantity)
    {
        if (quantity > ReservedStock)
            throw new InvalidOperationException("Onaylanacak miktar rezerve edilenden fazla olamaz.");

        Stock -= quantity;
        ReservedStock -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }
    
    // odeme basarisiz/rezervasyon suresi doldu
    // stoga hic dokunmadan sadece reervasyon geri alinir.
    public void ReleaseReservation(int quantity)
    {
        ReservedStock = Math.Max(0, ReservedStock - quantity);
        UpdatedAt = DateTime.UtcNow;
    }
    
}