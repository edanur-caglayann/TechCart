using TechCart.Products.Domain.Repositories;

namespace TechCart.Seeder;

public class ColorBackfiller
{
    private const int BatchSize = 200;

    // Uzun/birleşik renkler kontrol edilir ("rose gold", "gold"dan
    // önce), yoksa "gold" kelimesi "rose gold"u yanlış eşleştirebilirdi.
    private static readonly (string Keyword, string ColorName)[] KnownColorKeywords =
    {
        ("rose gold", "Rose Gold"),
        ("space grey", "Space Grey"),
        ("space gray", "Space Grey"),
        ("black", "Siyah"),
        ("white", "Beyaz"),
        ("blue", "Mavi"),
        ("red", "Kırmızı"),
        ("green", "Yeşil"),
        ("silver", "Gümüş"),
        ("gold", "Altın"),
        ("grey", "Gri"),
        ("gray", "Gri"),
        ("pink", "Pembe"),
        ("purple", "Mor"),
        ("yellow", "Sarı"),
        ("orange", "Turuncu"),
        ("brown", "Kahverengi"),
        ("beige", "Bej"),
        ("navy", "Lacivert"),
    };

    // Başlıkta hiç renk kelimesi geçmeyen ürünler için düşülecek sabit liste 
    private static readonly string[] FallbackColors = { "Siyah", "Beyaz", "Mavi", "Kırmızı", "Gri", "Gümüş" };

    private readonly IProductWriteRepository _productWriteRepository;
    public ColorBackfiller(IProductWriteRepository productWriteRepository) => _productWriteRepository = productWriteRepository;

    public async Task RunAsync(CancellationToken ct)
    {
        var products = await _productWriteRepository.GetProductsWithEmptyColorAsync(ct);
        Console.WriteLine($"Rengi boş {products.Count} ürün bulundu.");

        var extractedCount = 0;
        var randomCount = 0;
        var processedCount = 0;

        foreach (var product in products)
        {
            ct.ThrowIfCancellationRequested();

            var extractedColor = TryExtractColorFromTitle(product.Name);

            if (extractedColor is not null)
            {
                product.SetColor(extractedColor);
                extractedCount++;
            }
            else
            {
                var randomColor = FallbackColors[Random.Shared.Next(FallbackColors.Length)];
                product.SetColor(randomColor);
                randomCount++;
            }

            processedCount++;

            if (processedCount % BatchSize == 0)
            {
                await _productWriteRepository.SaveChangesAsync(ct);
                Console.WriteLine($"{processedCount} ürün işlendi...");
            }
        }

        await _productWriteRepository.SaveChangesAsync(ct);

        Console.WriteLine($"Tamamlandı: {extractedCount} ürün başlıktan çıkarıldı, {randomCount} ürüne rastgele renk atandı.");
    }

    private static string? TryExtractColorFromTitle(string title)
    {
        var lowerTitle = title.ToLowerInvariant();

        foreach (var (keyword, colorName) in KnownColorKeywords)
        {
            if (lowerTitle.Contains(keyword))
                return colorName;
        }

        return null;
    }
}