namespace TechCart.Inventory.Application.Dtos.ResponseDtos;

public record ProductStockDto(int Stock, bool InStock, bool IsReadyToShip, bool HasFastDelivery);
