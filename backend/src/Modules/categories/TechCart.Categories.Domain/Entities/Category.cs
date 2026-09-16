using TechCart.SharedKernel.Entities;

namespace TechCart.Categories.Domain.Entities;

public sealed class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    private Category() { }

    private Category(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
    public static Category Create(string name) => new(name);
}
