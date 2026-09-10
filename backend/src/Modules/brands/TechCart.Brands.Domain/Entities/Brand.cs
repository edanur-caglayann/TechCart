using TechCart.SharedKernel.Entities;

namespace TechCart.Brands.Domain.Entities;

public sealed class Brand : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    private Brand() { } 

    private Brand(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    public static Brand Create(string name) => new(name);
}
