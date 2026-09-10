using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCart.Brands.Domain.Entities;

namespace TechCart.Brands.Infrastructure.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("brands");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name).HasColumnName("name").HasMaxLength(150).IsRequired();

        builder.HasIndex(b => b.Name).IsUnique();
    }
}