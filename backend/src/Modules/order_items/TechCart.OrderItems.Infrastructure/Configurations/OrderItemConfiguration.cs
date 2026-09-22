using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCart.OrderItems.Domain.Entities;

namespace TechCart.OrderItems.Infrastructure.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id");
        builder.Property(i => i.OrderId).HasColumnName("order_id").IsRequired();
        builder.Property(i => i.ProductId).HasColumnName("product_id"); // nullable, IsRequired YOK
        builder.Property(i => i.ProductName).HasColumnName("product_name").IsRequired();
        builder.Property(i => i.ProductModel).HasColumnName("product_model");
        builder.Property(i => i.Quantity).HasColumnName("quantity").IsRequired();
        builder.Property(i => i.UnitPrice).HasColumnName("unit_price");
        builder.Property(i => i.VatRate).HasColumnName("vat_rate");
        builder.Property(i => i.VatAmount).HasColumnName("vat_amount");

        builder.HasIndex(i => i.OrderId);
    }
}