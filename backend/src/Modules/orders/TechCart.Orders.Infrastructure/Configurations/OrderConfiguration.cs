using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCart.Orders.Domain.Entities;

namespace TechCart.Orders.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnName("id");
        builder.Property(o => o.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(o => o.OrderNumber).HasColumnName("order_number").IsRequired();
        // HasConversion<string>() ile enum'u db'de sayi olarak degilir okunabilir metin olarak saklariz.
        builder.Property(o => o.Status).HasColumnName("status").HasConversion<string>().IsRequired();
        builder.Property(o => o.Subtotal).HasColumnName("subtotal");
        builder.Property(o => o.VatTotal).HasColumnName("vat_total");
        builder.Property(o => o.ShippingFee).HasColumnName("shipping_fee");
        builder.Property(o => o.Total).HasColumnName("total");
        builder.Property(o => o.ShippingFullName).HasColumnName("shipping_full_name");
        builder.Property(o => o.ShippingPhone).HasColumnName("shipping_phone");
        builder.Property(o => o.ShippingCity).HasColumnName("shipping_city");
        builder.Property(o => o.ShippingDistrict).HasColumnName("shipping_district");
        builder.Property(o => o.ShippingAddressLine).HasColumnName("shipping_address_line");
        builder.Property(o => o.ShippingPostalCode).HasColumnName("shipping_postal_code");
        builder.Property(o => o.CreatedAt).HasColumnName("created_at");
        builder.Property(o => o.UpdatedAt).HasColumnName("updated_at");

        // OrderNumber, kullanıcıya gösterilen/aranacak bir alan olduğu için
        builder.HasIndex(o => o.OrderNumber).IsUnique();
    }
}