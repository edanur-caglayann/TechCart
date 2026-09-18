namespace TechCart.Api.Controllers.Cart.Dtos.RequestDtos;

// frontend'den urun id + adedi gelir
public class MergeCartItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}