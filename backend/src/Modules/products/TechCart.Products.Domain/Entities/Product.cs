using TechCart.SharedKernel;
using TechCart.SharedKernel.Entities;

namespace TechCart.Products.Domain.Entities;

public class Product : AuditableEntity
{
    public Guid CategoryId { get; private set; }
    public Guid BrandId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Model { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string Specs { get; private set; } = default!;
    public string Color { get; private set; } = default!;
    public decimal Price { get; private set; }
    public decimal VatRate { get; private set; }

    private Product() { } 

    private Product(Guid categoryId, Guid brandId, string name, string model, string description,
        string specs, string color, decimal price, decimal vatRate)
    {
        Id = Guid.NewGuid();
        CategoryId = categoryId;
        BrandId = brandId;
        Name = name;
        Model = model;
        Description = description;
        Specs = specs;
        Color = color;
        Price = price;
        VatRate = vatRate;
        CreatedAt = DateTime.UtcNow;
    }
    
    public static Product Create(Guid categoryId, Guid brandId, string name, string model,
        string description, string specs, string color, decimal price, decimal vatRate)
        => new(categoryId, brandId, name, model, description, specs, color, price, vatRate);
}