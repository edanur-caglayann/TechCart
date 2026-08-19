import {
    Headphones,
  Laptop,
  Smartphone,
  Star,
  Tablet,
  Tv,
  Disc3,
} from "lucide-react";

// sayfalar arasinda yonlendirme yapmak icin Next.js'in Link bilesenini 
import Link from "next/link";
// urun hangi alanlara sahip
import { Product } from "../../types/product";
import styles from "./ProductCard.module.css";

// product karti icin gerekli olan propslari tanimlar
type ProductCardProps = {
  product: Product;
};

// urunun kategorisine gore ikonunu gostermek icin kullanilan fonksiyon
function ProductVisual({
    type,
}: {
    // type degeri Product icindeki visualType alanindan gelir
    type: Product["visualType"];
}) 

{
    // viualType degeri phone ise telefon ikonunu gostermek icin Lucide-react kütüphanesinden Smartphone ikonunu kullan
    if (type === "phone") {
        return <Smartphone size={95} strokeWidth={1.2} />;
    }

    if (type === "computer") {
    return <Laptop size={120} strokeWidth={1.2} />;
    }

    if (type === "tablet") {
    return <Tablet size={100} strokeWidth={1.2} />;
    }

     if (type === "television") {
    return <Tv size={120} strokeWidth={1.2} />;
    }

     if (type === "headphone") {
    return <Headphones size={105} strokeWidth={1.2} />;
     }

    // Yukarıdaki türlerden biri değilse varsayılan olarak
    // pikabı temsil eden disk ikonu gösterilir.
    return <Disc3 size={105} strokeWidth={1.2} />;
}

// Tek bir ürün kartını oluşturan ana bileşen.
    export default function ProductCard({
     product,
    }: ProductCardProps) {
  return (
    // article, tek başına anlamlı bir içeriği temsil eder.
    // Her ürün bağımsız bir içerik olduğu için article kullanıyoruz.
    <article className={styles.card}>
      
      <div className={styles.visual}>
        <ProductVisual type={product.visualType} />

        {!product.inStock && (
          <span className={styles.outOfStockBadge}>
            Tükendi
          </span>
        )}
      </div>

      <div className={styles.content}>
        <div className={styles.productMeta}>
          <span className={styles.brand}>
            {product.brand}
          </span>

          <span className={styles.separator}>•</span>

          <span className={styles.category}>
            {product.category}
          </span>
        </div>

        <h3 className={styles.productName}>
          {product.name}
        </h3>

        <p className={styles.model}>
          Model: {product.model}
        </p>

        <div className={styles.ratingArea}>
          <div className={styles.rating}>
            <Star
              size={17}

              fill="currentColor"

              strokeWidth={1.5}
            />

            <span>{product.rating}</span>
          </div>

          <span className={styles.reviewCount}>
            ({product.reviewCount} değerlendirme)
          </span>
        </div>

        <strong className={styles.price}>
          {/*
            toLocaleString("tr-TR"), fiyatı Türkçe sayı
            biçimine dönüştürür.

            Örnek:
            89999 → 89.999
          */}
          {product.price.toLocaleString("tr-TR")} ₺
        </strong>

        <div className={styles.actions}>
          {/*
            Ürün detay sayfasına yönlendiren bağlantı.

            product.id değeri 5 ise oluşacak adres:
            /products/5
          */}
          <Link
            className={styles.detailsButton}
            href={`/products/${product.id}`}
          >
            Detayları Gör
          </Link>

          {/* Ürünü sepete eklemek için kullanılacak buton */}
          <button
            className={styles.cartButton}

            type="button"

            // Ürün stokta değilse butonu devre dışı bırakır.
            disabled={!product.inStock}
          >
    
            {product.inStock ? "Sepete Ekle" : "Tükendi"}
          </button>
        </div>
      </div>
    </article>
  );
}

