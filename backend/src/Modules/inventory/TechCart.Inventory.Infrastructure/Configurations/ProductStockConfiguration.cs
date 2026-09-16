using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCart.Inventory.Domain.Entities;

namespace TechCart.Inventory.Infrastructure.Configurations;

public class ProductStockConfiguration : IEntityTypeConfiguration<ProductStock>
{
    public void Configure(EntityTypeBuilder<ProductStock> builder)
    {
        builder.ToTable("product_stock");

        builder.HasKey(s => s.ProductId);
        builder.Property(s => s.ProductId).HasColumnName("product_id").ValueGeneratedNever();

        builder.Property(s => s.Stock).HasColumnName("stock");
        builder.Property(s => s.IsReadyToShip).HasColumnName("is_ready_to_ship");
        builder.Property(s => s.HasFastDelivery).HasColumnName("has_fast_delivery");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
    }
}