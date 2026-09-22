using MassTransit;
using TechCart.EventBus.Events;
using TechCart.Products.Application.UpdateStock;

namespace TechCart.Products.Infrastructure.Consumers;

public class ProductStockChangedConsumer(UpdateProductStockHandler updateProductStockHandler)
    : IConsumer<ProductStockChangedEvent> // bu arayuz ile MassTransit'e ben bu tip mesaj dinlemek istiyorum diyrouz
{
    // consume metodu, RabbitMQ'dan bu tip bir mesaj geldiginde otomatik olarak calisir.
    // Biz elle cagirmayiz, MassTransit cagirir.
    public async Task Consume(ConsumeContext<ProductStockChangedEvent> context)
    {
        // mesajdaki bilgiyi alip (ProductId, NewStock) UpdateProductStockHandler fonksiyonuna iletiriz
        var message = context.Message;

        await updateProductStockHandler.Handle(
            new UpdateProductStockCommand(message.ProductId, message.NewStock), context.CancellationToken);
    }
}