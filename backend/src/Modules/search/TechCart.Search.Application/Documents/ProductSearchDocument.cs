namespace TechCart.Search.Application.Documents;

// Elasticsearch'e yazılacak, ürünün arama kopyası. postgres'teki bir tablo satirinin
// elk'daki karsiligi

// urun dokumani hangi alanlara sahip olacak
public class ProductSearchDocument
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SearchText { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool InStock { get; set; }
    public string? Image { get; set; }
    public DateTime CreatedAt { get; set; }
}