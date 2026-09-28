using TechCart.Products.Contracts.Dtos.ResponseDtos;

namespace TechCart.Search.Application.Documents;

//product modulunden gelen urunu elk'ya gondermek icin uygun dokuman formatina cevirir. 
//buna mapper deriz
public static class ProductSearchDocumentMapper
{
    // ToDocument -> Mapper girdisini alip ciktiisni ureten hazir fonks
    public static ProductSearchDocument ToDocument(ProductIndexItemResponse item) => new()
    {
        Id = item.Id.ToString(), // elk'da id alanini metin tanimaldigimiz icin 
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

    // Aranabilir dort alani tek bir metinde birleştirir. önce ürün adını listeye ekler.
    // marka zaten adin icinde oldugu icin marka alanini tekrar eklemez
    private static string BuildSearchText(ProductIndexItemResponse item)
    {
        var parts = new List<string> { item.Name };
        
        // StringComparison.OrdinalIgnoreCase -> büyük/küçük harfi ve kültürü yok sayarak karşılaştırır
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