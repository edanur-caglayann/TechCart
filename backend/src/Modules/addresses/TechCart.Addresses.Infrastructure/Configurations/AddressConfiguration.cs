using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCart.Addresses.Domain.Entities;

namespace TechCart.Addresses.Infrastructure.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(a => a.Title).HasColumnName("title").HasMaxLength(100).IsRequired();
        builder.Property(a => a.FullName).HasColumnName("full_name").HasMaxLength(200).IsRequired();
        builder.Property(a => a.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
        builder.Property(a => a.City).HasColumnName("city").HasMaxLength(100).IsRequired();
        builder.Property(a => a.District).HasColumnName("district").HasMaxLength(100).IsRequired();
        builder.Property(a => a.Neighborhood).HasColumnName("neighborhood").HasMaxLength(150).IsRequired();
        builder.Property(a => a.AddressLine).HasColumnName("address_line").IsRequired();
        builder.Property(a => a.PostalCode).HasColumnName("postal_code").HasMaxLength(20).IsRequired();
        builder.Property(a => a.IsDefault).HasColumnName("is_default");
        builder.Property(a => a.CreatedAt).HasColumnName("created_at");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");

        // user_id: "bu kullanıcının tüm adresleri" sorgusu her CRUD işleminde çalışıyor,
        builder.HasIndex(a => a.UserId);
    }
}