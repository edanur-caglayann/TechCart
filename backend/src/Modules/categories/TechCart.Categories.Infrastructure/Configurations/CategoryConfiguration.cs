using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCart.Categories.Domain.Entities;

namespace TechCart.Categories.Infrastructure.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(150).IsRequired();

        // INDEX: isim üzerinde unique. Seed script'in GetByNameAsync'i bunu kullanir
        builder.HasIndex(c => c.Name).IsUnique();
    }
}