using TechCart.SharedKernel;
using TechCart.SharedKernel.Entities;

namespace TechCart.Addresses.Domain.Entities;

public class Address : AuditableEntity
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string District { get; private set; } = default!;
    public string Neighborhood { get; private set; } = default!;
    public string AddressLine { get; private set; } = default!;
    public string PostalCode { get; private set; } = default!;
    public bool IsDefault { get; private set; }

    private Address() { } 

    private Address(Guid userId, string title, string fullName, string phone,
        string city, string district, string neighborhood, string addressLine, string postalCode, bool isDefault)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Title = title;
        FullName = fullName;
        Phone = phone;
        City = city;
        District = district;
        Neighborhood = neighborhood;
        AddressLine = addressLine;
        PostalCode = postalCode;
        IsDefault = isDefault;
        CreatedAt = DateTime.UtcNow;
    }
    
    // Domain'in kendisi "bu kullanıcının ilk adresi mi" diye veritabanına bakamaz,Karari handler verip buraya sonucu tasir
    public static Address Create(Guid userId, string title, string fullName, string phone,
        string city, string district, string neighborhood, string addressLine, string postalCode, bool isDefault)
        => new(userId, title, fullName, phone, city, district,neighborhood, addressLine, postalCode, isDefault);

    public void UpdateDetails(string title, string fullName, string phone,
        string city, string district, string neighborhood, string addressLine, string postalCode)
    {
        Title = title;
        FullName = fullName;
        Phone = phone;
        City = city;
        District = district;
        Neighborhood = neighborhood;
        AddressLine = addressLine;
        PostalCode = postalCode;
        MarkUpdated();
    }

    // adresi varsayilan adres yapar
    public void MarkAsDefault()
    {
        IsDefault = true;
        MarkUpdated();
    }

    // adresin varsayilan olma durumunu kaldirir
    public void UnmarkAsDefault()
    {
        IsDefault = false;
        MarkUpdated();
    }
}