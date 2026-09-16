using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCart.Users.Domain.Entities;

namespace TechCart.Users.Infrastructure.Configurations;

public class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
{
    public void Configure(EntityTypeBuilder<RevokedToken> builder)
    {
        builder.ToTable("revoked_tokens");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Jti).HasColumnName("jti").HasMaxLength(100).IsRequired();
        builder.Property(t => t.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(t => t.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.Property(t => t.RevokedAt).HasColumnName("revoked_at").IsRequired();

        builder.HasIndex(t => t.Jti).IsUnique();
        builder.HasIndex(t => t.ExpiresAt);
    }
}
