using TechCart.Inventory.Application.Abstractions;
using TechCart.Products.Domain.Repositories;

namespace TechCart.Seeder;

// mevcut kayıtları TRACKED ile çek,
// başka bir kaynaktan gelen bilgiyle güncelle, gruplar halinde kaydet.
public class ProductStockReplicator(
    IProductWriteRepository productWriteRepository,
    IProductStockReadRepository productStockReadRepository)
{
    private const int BatchSize = 200;

    public async Task RunAsync(CancellationToken ct)
    {
        var products = await productWriteRepository.GetAllAsync(ct);
        var stocksByProductId = await productStockReadRepository.GetAllStocksAsync(ct);

        Console.WriteLine($"{products.Count} ürün, {stocksByProductId.Count} stok kaydı bulundu.");

        var updatedCount = 0;
        var missingCount = 0;
        var processedCount = 0;

        foreach (var product in products)
        {
            ct.ThrowIfCancellationRequested();

            if (stocksByProductId.TryGetValue(product.Id, out var stock))
            {
                product.SetStock(stock);
                updatedCount++;
            }
            else
            {
                missingCount++;
            }

            processedCount++;

            if (processedCount % BatchSize == 0)
            {
                await productWriteRepository.SaveChangesAsync(ct);
                Console.WriteLine($"{processedCount} ürün işlendi...");
            }
        }

        await productWriteRepository.SaveChangesAsync(ct);

        Console.WriteLine($"Tamamlandı: {updatedCount} ürün güncellendi, {missingCount} ürünün Inventory'de kaydı yoktu.");
    }
}