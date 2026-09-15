using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using TechCart.Brands.Application.Abstractions;
using TechCart.Brands.Domain.Entities;
using TechCart.Brands.Domain.Repositories;
using TechCart.Categories.Application.Abstractions;
using TechCart.Categories.Domain.Entities;
using TechCart.Categories.Domain.Repositories;
using TechCart.Inventory.Domain.Entities;
using TechCart.Inventory.Domain.Repositories;
using TechCart.ProductImages.Domain.Entities;
using TechCart.ProductImages.Domain.Repositories;
using TechCart.Products.Domain.Entities;
using TechCart.Products.Domain.Repositories;

namespace TechCart.Seeder;

public class ProductSeeder
{
    private const int BatchSize = 200;
    private const decimal DefaultVatRate = 0.20m;

    // Bunun yerine ürün başlığında geçen anahtar kelimeye göre kategori çıkarıyoruz.
    private static readonly (string Keyword, string CategoryName)[] CategoryKeywords =
    {
        ("laptop", "Bilgisayar"), ("notebook", "Bilgisayar"), ("macbook", "Bilgisayar"),
        ("chromebook", "Bilgisayar"), ("desktop", "Bilgisayar"),
        ("tablet", "Tablet"), ("ipad", "Tablet"),
        ("keyboard", "Klavye"),
        ("trackpad", "Mouse"), ("mouse", "Mouse"),
        ("turntable", "Pikap"), ("record player", "Pikap"), ("vinyl", "Pikap"),
        ("headphone", "Kulaklık"), ("earphone", "Kulaklık"), ("earbud", "Kulaklık"), ("headset", "Kulaklık"),
        ("soundbar", "Hoparlör"), ("speaker", "Hoparlör"),
        ("smart tv", "Televizyon"), ("television", "Televizyon"),
        ("smartwatch", "Akıllı Saat"), ("smart watch", "Akıllı Saat"), ("fitness band", "Akıllı Saat"),
        ("webcam", "Kamera"), ("dslr", "Kamera"), ("gopro", "Kamera"), ("camera", "Kamera"),
        ("smartphone", "Telefon"), ("mobile phone", "Telefon"),
    };

    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly ICategoryWriteRepository _categoryWriteRepository;
    private readonly IBrandReadRepository _brandReadRepository;
    private readonly IBrandWriteRepository _brandWriteRepository;
    private readonly IProductWriteRepository _productWriteRepository;
    private readonly IProductImageWriteRepository _productImageWriteRepository;
    private readonly IProductStockWriteRepository _productStockWriteRepository;

    public ProductSeeder(
        ICategoryReadRepository categoryReadRepository,
        ICategoryWriteRepository categoryWriteRepository,
        IBrandReadRepository brandReadRepository,
        IBrandWriteRepository brandWriteRepository,
        IProductWriteRepository productWriteRepository,
        IProductImageWriteRepository productImageWriteRepository,
        IProductStockWriteRepository productStockWriteRepository)
    {
        _categoryReadRepository = categoryReadRepository;
        _categoryWriteRepository = categoryWriteRepository;
        _brandReadRepository = brandReadRepository;
        _brandWriteRepository = brandWriteRepository;
        _productWriteRepository = productWriteRepository;
        _productImageWriteRepository = productImageWriteRepository;
        _productStockWriteRepository = productStockWriteRepository;
    }

    public async Task RunAsync(string csvPath, CancellationToken ct)
    {
        var categoryIdsByName = (await _categoryReadRepository.GetAllAsync(ct))
            .ToDictionary(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase);

        var brandIdsByName = (await _brandReadRepository.GetAllAsync(ct))
            .ToDictionary(b => b.Name, b => b.Id, StringComparer.OrdinalIgnoreCase);

        var productIdsByName = await _productWriteRepository.GetAllProductIdsByNameAsync(ct);
        var productIdsWithStock = await _productStockWriteRepository.GetAllProductIdsAsync(ct);

        Console.WriteLine($"Başlangıç: {categoryIdsByName.Count} kategori, {brandIdsByName.Count} marka, " +
            $"{productIdsByName.Count} ürün, {productIdsWithStock.Count} stok kaydı zaten var.");

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<AmazonElectronicsCsvRow>();

        var newProductCount = 0;
        var newStockCount = 0;
        var skippedEmptyTitleCount = 0;
        var skippedNoCategoryMatchCount = 0; // kaç ürünün "teknolojik alet değil" diye atlandığını görmek için

        var rowsHandled = 0;

        foreach (var row in records)
        {
            ct.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(row.Title))
            {
                skippedEmptyTitleCount++;
                continue;
            }

            Guid productId;

            if (productIdsByName.TryGetValue(row.Title, out var existingProductId))
            {
                productId = existingProductId;
            }
            else
            {
                var inferredCategory = TryInferCategory(row.Title);
                if (inferredCategory is null)
                {
                    skippedNoCategoryMatchCount++;
                    continue;
                }

                var categoryId = await GetOrCreateCategoryAsync(inferredCategory, categoryIdsByName, ct);
                var brandName = ExtractBrandName(row.Title);
                var brandId = await GetOrCreateBrandAsync(brandName, brandIdsByName, ct);

                var discountPrice = ParseDecimal(row.DiscountPrice);
                var actualPrice = ParseDecimal(row.ActualPrice);
                var price = discountPrice > 0 ? discountPrice : actualPrice;

                var product = Product.Create(categoryId, brandId, row.Title, string.Empty,
                    string.Empty, "{}", string.Empty, price, DefaultVatRate);

                await _productWriteRepository.AddAsync(product, ct);
                productIdsByName[row.Title] = product.Id;
                productId = product.Id;
                newProductCount++;

                if (!string.IsNullOrWhiteSpace(row.ImageUrl))
                {
                    var image = ProductImage.Create(product.Id, row.ImageUrl, sortOrder: 0);
                    await _productImageWriteRepository.AddAsync(image, ct);
                }
            }

            if (!productIdsWithStock.Contains(productId))
            {
                var stock = ProductStock.Create(
                    productId,
                    stock: Random.Shared.Next(0, 101),
                    isReadyToShip: Random.Shared.Next(0, 100) < 80,
                    hasFastDelivery: Random.Shared.Next(0, 100) < 50);

                await _productStockWriteRepository.AddAsync(stock, ct);
                productIdsWithStock.Add(productId);
                newStockCount++;
            }

            rowsHandled++;

            if (rowsHandled % BatchSize == 0)
            {
                await SaveAllAsync(ct);
                Console.WriteLine($"{rowsHandled} satır işlendi... ({newProductCount} yeni ürün, {newStockCount} yeni stok kaydı)");
            }
        }

        await SaveAllAsync(ct);

        Console.WriteLine($"Tamamlandı: {newProductCount} yeni ürün, {newStockCount} yeni stok kaydı eklendi. " +
            $"Atlanan: {skippedEmptyTitleCount} boş isim, {skippedNoCategoryMatchCount} kategori eşleşmedi.");
    }

    // başlıkta CategoryKeywords listesindeki ilk eşleşen anahtar kelimeyi arar, karşılık gelen kategori adını döner. 
    // Hiçbiri eşleşmezse null döner
    private static string? TryInferCategory(string title)
    {
        var lowerTitle = title.ToLowerInvariant();

        foreach (var (keyword, categoryName) in CategoryKeywords)
        {
            if (lowerTitle.Contains(keyword))
                return categoryName;
        }

        return null;
    }

    private async Task<Guid> GetOrCreateCategoryAsync(string categoryName, Dictionary<string, Guid> cache, CancellationToken ct)
    {
        if (cache.TryGetValue(categoryName, out var existingId))
            return existingId;

        var category = Category.Create(categoryName);
        await _categoryWriteRepository.AddAsync(category, ct);
        cache[categoryName] = category.Id;
        return category.Id;
    }

    private async Task<Guid> GetOrCreateBrandAsync(string? brandName, Dictionary<string, Guid> cache, CancellationToken ct)
    {
        var name = string.IsNullOrWhiteSpace(brandName) ? "Bilinmeyen Marka" : brandName.Trim();

        if (cache.TryGetValue(name, out var existingId))
            return existingId;

        var brand = Brand.Create(name);
        await _brandWriteRepository.AddAsync(brand, ct);
        cache[name] = brand.Id;
        return brand.Id;
    }

    private static decimal ParseDecimal(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return 0;

        var cleaned = Regex.Replace(raw, @"[^\d.]", "");
        return decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : 0;
    }

    private static string ExtractBrandName(string title)
    {
        var firstWord = title.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.IsNullOrWhiteSpace(firstWord) ? "Bilinmeyen Marka" : firstWord;
    }

    private async Task SaveAllAsync(CancellationToken ct)
    {
        await _categoryWriteRepository.SaveChangesAsync(ct);
        await _brandWriteRepository.SaveChangesAsync(ct);
        await _productWriteRepository.SaveChangesAsync(ct);
        await _productImageWriteRepository.SaveChangesAsync(ct);
        await _productStockWriteRepository.SaveChangesAsync(ct);
    }
}