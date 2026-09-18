namespace TechCart.Api.Controllers.Cart.Dtos.RequestDtos;

public class MergeCartRequest
{
    public List<MergeCartItemRequest> Items { get; set; } = [];
}