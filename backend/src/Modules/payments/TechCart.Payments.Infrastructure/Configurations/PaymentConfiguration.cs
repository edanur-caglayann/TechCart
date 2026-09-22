using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCart.Payments.Domain.Entities;

namespace TechCart.Payments.Infrastructure.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.OrderId).HasColumnName("order_id").IsRequired();
        builder.Property(p => p.Provider).HasColumnName("provider").IsRequired();
        builder.Property(p => p.Status).HasColumnName("status").HasConversion<string>().IsRequired();
        builder.Property(p => p.Amount).HasColumnName("amount");
        builder.Property(p => p.Currency).HasColumnName("currency");
        builder.Property(p => p.ProviderReference).HasColumnName("provider_reference").IsRequired();
        builder.Property(p => p.TdsReference).HasColumnName("tds_reference");
        builder.Property(p => p.FailureReason).HasColumnName("failure_reason");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        // ayni odeme islemi iki kez kaydedilmesin.
        // ProviderReference, iyzico gibi odeme araclarinin her odeme islemine verdi id'dir.
        // IsUnique() ile veritabani gelen ikinci kaydi kabul etmez.  
        builder.HasIndex(p => p.ProviderReference).IsUnique();

        // Bir OrderId için birden fazla Payment satırı olabildiği
        // (kullanıcı farklı kartla tekrar dener) için OrderId 
        // unique degil. Sadece sorguyu hizlandirir.
        builder.HasIndex(p => p.OrderId);
    }
}