using TechCart.SharedKernel;
using TechCart.SharedKernel.Entities;

namespace TechCart.CartItems.Domain.Entities;

public class CartItem : AuditableEntity
{
    public Guid UserId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }

    private CartItem() { } 

    private CartItem(Guid userId, Guid productId, int quantity)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ProductId = productId;
        Quantity = quantity;
        CreatedAt = DateTime.UtcNow;
    }

    public static CartItem Create(Guid userId, Guid productId, int quantity)
    {
        if (quantity < 1)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Sepete eklenecek adet en az 1 olmalıdır.");

        return new CartItem(userId, productId, quantity);
    }

    // Hem birleştirme (merge) sırasındaki "adetleri topla" hem de aynı ürün tekrar "sepete ekle" ile eklenirse kullanılacak.
    public void IncreaseQuantity(int amount)
    {
        if (amount < 1)
            throw new ArgumentOutOfRangeException(nameof(amount), "Artırılacak adet en az 1 olmalıdır.");

        Quantity += amount;
        MarkUpdated();
    }

    // Adet güncelleme (PATCH) endpoint'i bu metodu kullanacak.
    public void SetQuantity(int quantity)
    {
        if (quantity < 1)
            throw new ArgumentOutOfRangeException(nameof(quantity),
                "Sepetteki adet en az 1 olmalıdır — 0 veya altı için ürünü kaldırma işlemi kullanılmalı.");

        Quantity = quantity;
        MarkUpdated();
    }
}