using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCart.CartItems.Domain.Entities;

namespace TechCart.CartItems.Infrastructure.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("cart_items");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(c => c.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(c => c.Quantity).HasColumnName("quantity").IsRequired();
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");

        // Aynı kullanıcının aynı ürünü sepette iki ayri satırda tutmasıni veritabanı seviyesinde engelliyoruz 
        // birleştirme mantığının da temeli.
        builder.HasIndex(c => new { c.UserId, c.ProductId }).IsUnique();
    }
}