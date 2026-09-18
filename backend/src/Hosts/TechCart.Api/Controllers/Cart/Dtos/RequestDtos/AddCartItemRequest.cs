namespace TechCart.Api.Controllers.Cart.Dtos.RequestDtos;

public class AddCartItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}