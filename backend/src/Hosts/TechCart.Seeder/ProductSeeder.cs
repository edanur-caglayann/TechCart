using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using TechCart.Brands.Application.Abstractions;
using TechCart.Brands.Domain.Entities;
using TechCart.Brands.Domain.Repositories;
using TechCart.Categories.Application.Abstractions;
using TechCart.Categories.Domain.Entities;
using TechCart.Categories.Domain.Repositories;
using TechCart.ProductImages.Domain.Entities;
using TechCart.ProductImages.Domain.Repositories;
using TechCart.Products.Domain.Entities;
using TechCart.Products.Domain.Repositories;

namespace TechCart.Seeder;

// CSV'deki ürünleri okuyup Postgres'e (Category/Brand/Product/ProductImage
// tablolarına) toplu şekilde yazan, tek seferlik çalıştırılan sınıf.
public class ProductSeeder
{
    // Kaç üründe bir SaveChanges çağrılacağı — tek tek değil, gruplar halinde
    // kaydederek veritabanına gidiş-geliş sayısını azaltıyoruz.
    private const int BatchSize = 200;

    // Veri setinde Türkiye KDV bilgisi olmadığı için sabit oran kullanıyoruz.
    private const decimal DefaultVatRate = 0.20m;

    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly ICategoryWriteRepository _categoryWriteRepository;
    private readonly IBrandReadRepository _brandReadRepository;
    private readonly IBrandWriteRepository _brandWriteRepository;
    private readonly IProductWriteRepository _productWriteRepository;
    private readonly IProductImageWriteRepository _productImageWriteRepository;

    public ProductSeeder(
        ICategoryReadRepository categoryReadRepository,
        ICategoryWriteRepository categoryWriteRepository,
        IBrandReadRepository brandReadRepository,
        IBrandWriteRepository brandWriteRepository,
        IProductWriteRepository productWriteRepository,
        IProductImageWriteRepository productImageWriteRepository)
    {
        _categoryReadRepository = categoryReadRepository;
        _categoryWriteRepository = categoryWriteRepository;
        _brandReadRepository = brandReadRepository;
        _brandWriteRepository = brandWriteRepository;
        _productWriteRepository = productWriteRepository;
        _productImageWriteRepository = productImageWriteRepository;
    }

    public async Task RunAsync(string csvPath, CancellationToken ct)
    {
        // Mevcut kategori/marka/ürün isimlerini TEK seferde belleğe çekiyoruz —
        // döngü içinde her satırda ayrı ayrı veritabanına sormuyoruz.
        var categoryIdsByName = (await _categoryReadRepository.GetAllAsync(ct))
            .ToDictionary(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase);

        var brandIdsByName = (await _brandReadRepository.GetAllAsync(ct))
            .ToDictionary(b => b.Name, b => b.Id, StringComparer.OrdinalIgnoreCase);

        var existingProductNames = await _productWriteRepository.GetAllNamesAsync(ct);

        Console.WriteLine($"Başlangıç: {categoryIdsByName.Count} kategori, {brandIdsByName.Count} marka, {existingProductNames.Count} ürün zaten kayıtlı.");

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<AmazonElectronicsCsvRow>();

        var processedCount = 0;
        var skippedCount = 0;

        foreach (var row in records)
        {
            ct.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(row.Title))
            {
                skippedCount++;
                continue; // ürün adı zorunlu, boşsa satırı atla
            }

            // Script tekrar çalıştırılırsa aynı ürün ikinci kez eklenmesin.
            if (existingProductNames.Contains(row.Title))
            {
                skippedCount++;
                continue;
            }

            var categoryId = await GetOrCreateCategoryAsync(row.Category, categoryIdsByName, ct);

            // Marka kolonu yok — ürün adının ilk kelimesini marka olarak kullanıyoruz.
            var brandName = ExtractBrandName(row.Title);
            var brandId = await GetOrCreateBrandAsync(brandName, brandIdsByName, ct);

            // Önce indirimli fiyatı dene, yoksa orijinal (MRP) fiyata düş.
            var discountPrice = ParseDecimal(row.DiscountPrice);
            var actualPrice = ParseDecimal(row.ActualPrice);
            var price = discountPrice > 0 ? discountPrice : actualPrice;

            var product = Product.Create(
                categoryId,
                brandId,
                name: row.Title,
                model: string.Empty,
                description: string.Empty,
                specs: "{}",
                color: string.Empty,
                price: price,
                vatRate: DefaultVatRate);

            await _productWriteRepository.AddAsync(product, ct);
            existingProductNames.Add(product.Name); // aynı çalıştırma içindeki tekrarı da engeller

            if (!string.IsNullOrWhiteSpace(row.ImageUrl))
            {
                var image = ProductImage.Create(product.Id, row.ImageUrl, sortOrder: 0);
                await _productImageWriteRepository.AddAsync(image, ct);
            }

            processedCount++;

            if (processedCount % BatchSize == 0)
            {
                await SaveAllAsync(ct);
                Console.WriteLine($"{processedCount} ürün işlendi...");
            }
        }

        await SaveAllAsync(ct); 

        Console.WriteLine($"Tamamlandı: {processedCount} ürün eklendi, {skippedCount} satır atlandı.");
    }

    // Kategori belleğimizdeki sözlükte varsa id'sini döner; yoksa yeni Category
    // oluşturup (henüz kaydetmeden, sadece AddAsync ile) sözlüğe ekler.
    private async Task<Guid> GetOrCreateCategoryAsync(string categoryName, Dictionary<string, Guid> cache, CancellationToken ct)
    {
        var name = string.IsNullOrWhiteSpace(categoryName) ? "Diğer" : categoryName.Trim();

        if (cache.TryGetValue(name, out var existingId))
            return existingId;

        var category = Category.Create(name);
        await _categoryWriteRepository.AddAsync(category, ct);
        cache[name] = category.Id;
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

    // Regex.Replace ile rakam/nokta disindaki her şeyi siliyoruz 
    private static decimal ParseDecimal(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return 0;

        var cleaned = Regex.Replace(raw, @"[^\d.]", "");
        return decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : 0;
    }
    
    // Amazon başlıkları neredeyse her zaman markayla başlar, o yüzden ilk kelimeyi marka olarak alıyoruz.
    private static string ExtractBrandName(string title)
    {
        var firstWord = title.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.IsNullOrWhiteSpace(firstWord) ? "Bilinmeyen Marka" : firstWord;
    }

    // Her modül kendi DbContext'ini kaydediyor — tek bir dev transaction yok,
    // "her modül kendi verisinden sorumlu" kuralımız burada da geçerli.
    private async Task SaveAllAsync(CancellationToken ct)
    {
        await _categoryWriteRepository.SaveChangesAsync(ct);
        await _brandWriteRepository.SaveChangesAsync(ct);
        await _productWriteRepository.SaveChangesAsync(ct);
        await _productImageWriteRepository.SaveChangesAsync(ct);
    }
}