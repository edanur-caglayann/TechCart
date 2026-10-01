using TechCart.Products.Contracts.Dtos.ResponseDtos;

namespace TechCart.Search.Application.Documents;

public static class ProductSearchDocumentMapper
{
    public static ProductSearchDocument ToDocument(ProductIndexItemResponse item) => new()
    {
        Id = item.Id.ToString(),
        Name = item.Name,
        SearchText = BuildSearchText(item),
        Brand = item.Brand,
        Category = item.Category,
        Color = item.Color,
        Price = item.Price,
        InStock = item.InStock,
        Image = item.Image,
        CreatedAt = item.CreatedAt
    };

    // Aranabilir alanları tek bir metinde birleştirir: "yeşil kulaklık" gibi bir
    // arama, farklı alanlarda duran kelimeleri tek sorguyla bulabilsin diye.
    private static string BuildSearchText(ProductIndexItemResponse item)
    {
        var parts = new List<string> { item.Name };

        // Amazon başlıkları markayla başlıyor ve markayı da başlığın ilk kelimesinden
        // türettik. Bu yüzden marka genellikle zaten adın içinde, aynı kelimeyi ikinci
        // kez yazmıyoruz.
        if (!string.IsNullOrWhiteSpace(item.Brand) &&
            !item.Name.Contains(item.Brand, StringComparison.OrdinalIgnoreCase))
            parts.Add(item.Brand);

        if (!string.IsNullOrWhiteSpace(item.Category))
            parts.Add(item.Category);

        if (!string.IsNullOrWhiteSpace(item.Color))
            parts.Add(item.Color);

        return string.Join(' ', parts.Select(p => p.Trim()));
    }
}