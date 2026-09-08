using Microsoft.EntityFrameworkCore;
using TechCart.SharedKernel.Entities;
using Address = TechCart.Addresses.Domain.Entities.Address;
using Brand = TechCart.Brands.Domain.Entities.Brand;
using CartItem = TechCart.CartItems.Domain.Entities.CartItem;
using Category = TechCart.Categories.Domain.Entities.Category;
using Order = TechCart.Orders.Domain.Entities.Order;
using OrderItem = TechCart.OrderItems.Domain.Entities.OrderItem;
using PaymentEntity = TechCart.Payments.Domain.Entities.Payment;
using Product = TechCart.Products.Domain.Entities.Product;
using ProductImage = TechCart.ProductImages.Domain.Entities.ProductImage;
using ProductStockEntity = TechCart.ProductStock.Domain.Entities.ProductStock;
using ShoppingCart = TechCart.Carts.Domain.Entities.Cart;
using User = TechCart.Users.Domain.Entities.User;

namespace TechCart.Infrastructure.Persistence;

public sealed class TechCartDbContext(DbContextOptions<TechCartDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Address> Addresses => Set<Address>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Brand> Brands => Set<Brand>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    public DbSet<ProductStockEntity> ProductStock => Set<ProductStockEntity>();

    public DbSet<ShoppingCart> Carts => Set<ShoppingCart>();

    public DbSet<CartItem> CartItems => Set<CartItem>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<PaymentEntity> Payments => Set<PaymentEntity>();

    public override int SaveChanges()
    {
        SetAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUsers(modelBuilder);
        ConfigureAddresses(modelBuilder);
        ConfigureCategories(modelBuilder);
        ConfigureBrands(modelBuilder);
        ConfigureProducts(modelBuilder);
        ConfigureProductImages(modelBuilder);
        ConfigureProductStock(modelBuilder);
        ConfigureCarts(modelBuilder);
        ConfigureCartItems(modelBuilder);
        ConfigureOrders(modelBuilder);
        ConfigureOrderItems(modelBuilder);
        ConfigurePayments(modelBuilder);
    }

    private void SetAuditFields()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = entry.Entity.CreatedAt == default ? utcNow : entry.Entity.CreatedAt;
                entry.Entity.UpdatedAt = entry.Entity.UpdatedAt == default ? utcNow : entry.Entity.UpdatedAt;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<CreationAuditedEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.CreatedAt == default)
            {
                entry.Entity.CreatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<ProductStockEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            entity.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(256).IsRequired();
            entity.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(512).IsRequired();
            entity.Property(x => x.Role).HasColumnName("role").HasMaxLength(50).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
        });
    }

    private static void ConfigureAddresses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("addresses");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(x => x.Title).HasColumnName("title").HasMaxLength(100).IsRequired();
            entity.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(30).IsRequired();
            entity.Property(x => x.City).HasColumnName("city").HasMaxLength(100).IsRequired();
            entity.Property(x => x.District).HasColumnName("district").HasMaxLength(100).IsRequired();
            entity.Property(x => x.AddressLine).HasColumnName("address_line").HasColumnType("text").IsRequired();
            entity.Property(x => x.PostalCode).HasColumnName("postal_code").HasMaxLength(20).IsRequired();
            entity.Property(x => x.IsDefault).HasColumnName("is_default").IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => x.UserId);
        });
    }

    private static void ConfigureCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
        });
    }

    private static void ConfigureBrands(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("brands");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
        });
    }

    private static void ConfigureProducts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.CategoryId).HasColumnName("category_id").IsRequired();
            entity.Property(x => x.BrandId).HasColumnName("brand_id").IsRequired();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Model).HasColumnName("model").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Description).HasColumnName("description").HasColumnType("text").IsRequired();
            entity.Property(x => x.Specs).HasColumnName("specs").HasColumnType("jsonb").IsRequired();
            entity.Property(x => x.Color).HasColumnName("color").HasMaxLength(80).IsRequired();
            entity.Property(x => x.Price).HasColumnName("price").HasPrecision(18, 2).IsRequired();
            entity.Property(x => x.VatRate).HasColumnName("vat_rate").HasPrecision(5, 2).IsRequired();
            entity.Property(x => x.Rating).HasColumnName("rating").HasPrecision(3, 2).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.HasOne<Category>().WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Brand>().WithMany().HasForeignKey(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => x.CategoryId);
            entity.HasIndex(x => x.BrandId);
        });
    }

    private static void ConfigureProductImages(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.ToTable("product_images");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.ProductId).HasColumnName("product_id").IsRequired();
            entity.Property(x => x.ImageUrl).HasColumnName("image_url").HasMaxLength(1000).IsRequired();
            entity.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();
            entity.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => x.ProductId);
        });
    }

    private static void ConfigureProductStock(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductStockEntity>(entity =>
        {
            entity.ToTable("product_stock");
            entity.HasKey(x => x.ProductId);
            entity.Property(x => x.ProductId).HasColumnName("product_id");
            entity.Property(x => x.Stock).HasColumnName("stock").IsRequired();
            entity.Property(x => x.IsReadyToShip).HasColumnName("is_ready_to_ship").IsRequired();
            entity.Property(x => x.HasFastDelivery).HasColumnName("has_fast_delivery").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.HasOne<Product>().WithOne().HasForeignKey<ProductStockEntity>(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureCarts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.ToTable("carts");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.GuestToken).HasColumnName("guest_token").HasMaxLength(256);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.HasOne<User>().WithOne().HasForeignKey<ShoppingCart>(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(x => x.UserId).IsUnique();
            entity.HasIndex(x => x.GuestToken).IsUnique();
        });
    }

    private static void ConfigureCartItems(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("cart_items");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.CartId).HasColumnName("cart_id").IsRequired();
            entity.Property(x => x.ProductId).HasColumnName("product_id").IsRequired();
            entity.Property(x => x.Quantity).HasColumnName("quantity").IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.HasOne<ShoppingCart>().WithMany().HasForeignKey(x => x.CartId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => x.CartId);
            entity.HasIndex(x => x.ProductId);
            entity.HasIndex(x => new { x.CartId, x.ProductId }).IsUnique();
        });
    }

    private static void ConfigureOrders(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(x => x.OrderNumber).HasColumnName("order_number").HasMaxLength(80).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Subtotal).HasColumnName("subtotal").HasPrecision(18, 2).IsRequired();
            entity.Property(x => x.VatTotal).HasColumnName("vat_total").HasPrecision(18, 2).IsRequired();
            entity.Property(x => x.ShippingFee).HasColumnName("shipping_fee").HasPrecision(18, 2).IsRequired();
            entity.Property(x => x.Total).HasColumnName("total").HasPrecision(18, 2).IsRequired();
            entity.Property(x => x.ShippingFullName).HasColumnName("shipping_full_name").HasMaxLength(200).IsRequired();
            entity.Property(x => x.ShippingPhone).HasColumnName("shipping_phone").HasMaxLength(30).IsRequired();
            entity.Property(x => x.ShippingCity).HasColumnName("shipping_city").HasMaxLength(100).IsRequired();
            entity.Property(x => x.ShippingDistrict).HasColumnName("shipping_district").HasMaxLength(100).IsRequired();
            entity.Property(x => x.ShippingAddressLine).HasColumnName("shipping_address_line").HasColumnType("text").IsRequired();
            entity.Property(x => x.ShippingPostalCode).HasColumnName("shipping_postal_code").HasMaxLength(20).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.OrderNumber).IsUnique();
        });
    }

    private static void ConfigureOrderItems(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_items");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.OrderId).HasColumnName("order_id").IsRequired();
            entity.Property(x => x.ProductId).HasColumnName("product_id");
            entity.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(200).IsRequired();
            entity.Property(x => x.ProductModel).HasColumnName("product_model").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Quantity).HasColumnName("quantity").IsRequired();
            entity.Property(x => x.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 2).IsRequired();
            entity.Property(x => x.VatRate).HasColumnName("vat_rate").HasPrecision(5, 2).IsRequired();
            entity.Property(x => x.VatAmount).HasColumnName("vat_amount").HasPrecision(18, 2).IsRequired();
            entity.HasOne<Order>().WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(x => x.OrderId);
            entity.HasIndex(x => x.ProductId);
        });
    }

    private static void ConfigurePayments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentEntity>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.OrderId).HasColumnName("order_id").IsRequired();
            entity.Property(x => x.Provider).HasColumnName("provider").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
            entity.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
            entity.Property(x => x.ProviderReference).HasColumnName("provider_reference").HasMaxLength(200).IsRequired();
            entity.Property(x => x.TdsReference).HasColumnName("tds_reference").HasMaxLength(200);
            entity.Property(x => x.FailureReason).HasColumnName("failure_reason").HasColumnType("text");
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone").IsRequired();
            entity.HasOne<Order>().WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => x.OrderId);
            entity.HasIndex(x => x.ProviderReference);
        });
    }
}
