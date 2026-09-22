namespace TechCart.EventBus.Events;

// Bu, "mesajın kendisi" — MassTransit bunu JSON'a çevirip RabbitMQ'ya
// gönderecek, dinleyen taraf da aynı şekli bekleyecek.
public record ProductStockChangedEvent(Guid ProductId, int NewStock);