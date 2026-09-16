using CsvHelper.Configuration.Attributes;

namespace TechCart.Seeder;
public class AmazonElectronicsCsvRow
{
    [Name("name")] public string Title { get; set; } = default!;
    [Name("sub_category")] public string Category { get; set; } = default!;
    [Name("image")] public string? ImageUrl { get; set; }
    [Name("discount_price")] public string? DiscountPrice { get; set; }
    [Name("actual_price")] public string? ActualPrice { get; set; }
}