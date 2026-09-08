using TechCart.SharedKernel.Entities;

namespace TechCart.Addresses.Domain.Entities;

public sealed class Address : AuditableEntity
{
    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public string AddressLine { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}
