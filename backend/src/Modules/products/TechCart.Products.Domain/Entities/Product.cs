using TechCart.SharedKernel.Entities;

namespace TechCart.Products.Domain.Entities;

public sealed class Product : AuditableEntity
{
    public Guid CategoryId { get; set; }

    public Guid BrandId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Specs { get; set; } = "{}";

    public string Color { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal VatRate { get; set; }

    public decimal Rating { get; set; }
}
