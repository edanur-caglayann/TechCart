using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCart.Products.Domain.Entities;

namespace TechCart.Products.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.CategoryId).HasColumnName("category_id").IsRequired();
        builder.Property(p => p.BrandId).HasColumnName("brand_id").IsRequired();
        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(300).IsRequired();
        builder.Property(p => p.Model).HasColumnName("model").HasMaxLength(150).IsRequired();
        builder.Property(p => p.Description).HasColumnName("description").IsRequired();
        builder.Property(p => p.Specs).HasColumnName("specs").IsRequired();
        builder.Property(p => p.Color).HasColumnName("color").HasMaxLength(100).IsRequired();
        builder.Property(p => p.Price).HasColumnName("price").HasColumnType("decimal(18,2)");
        builder.Property(p => p.VatRate).HasColumnName("vat_rate").HasColumnType("decimal(5,4)");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.Stock).HasColumnName("stock"); 
       
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.BrandId);
        builder.HasIndex(p => p.Name).IsUnique(); 
    }
}