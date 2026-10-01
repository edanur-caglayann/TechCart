using MassTransit;
using Microsoft.Extensions.Logging;
using TechCart.EventBus.Events;
using TechCart.Search.Application.Abstractions;

namespace TechCart.Search.Infrastructure.Consumers;

// IConsumer<ProductStockChangedEvent> ile hangi mesajin dinlenecegini belirtiriz
// productSearchIndex eleasticsearch guncellemesi yapacak olan servis
// logger guncelleme basarisizsa hatayi kaydeder
public class SearchProductStockChangedConsumer(IProductSearchIndex productSearchIndex,
    ILogger<SearchProductStockChangedConsumer> logger) : IConsumer<ProductStockChangedEvent>
{
    public async Task Consume(ConsumeContext<ProductStockChangedEvent> context)
    {
        // context.Message, stok değişikliği mesajının kendisi.
        // Stok adedi, stokta var mı bilgisine çevriliyor.
        var inStock = context.Message.NewStock > 0;

        try
        {
            // Mesajdaki ürünün Elasticsearch dokümanını bul ve InStock alanını hesapladığım değerle güncelle.
            await productSearchIndex.UpdateStockAsync(context.Message.ProductId, inStock, context.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Ürün {ProductId} için Elasticsearch stok güncellemesi başarısız oldu.",
                context.Message.ProductId);
        }
    }
}