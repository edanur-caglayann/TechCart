using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCart.ProductImages.Domain.Entities;

namespace TechCart.ProductImages.Infrastructure.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("product_images");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(i => i.ImageUrl).HasColumnName("image_url").IsRequired();
        builder.Property(i => i.SortOrder).HasColumnName("sort_order");

        builder.HasIndex(i => i.ProductId);
    }
}