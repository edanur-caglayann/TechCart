namespace TechCart.Inventory.Application.Dtos.RequestDtos;

public record ReserveStockCommand(Guid ProductId, int Quantity);