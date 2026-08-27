"use client";

import {
  Disc3,
  Headphones,
  Laptop,
  Minus,
  Plus,
  Smartphone,
  Tablet,
  Trash2,
  Tv,
} from "lucide-react";

// Sayfalar arasında yönlendirme yapmak için
// Next.js'in Link bileşenini kullanıyoruz.
import Link from "next/link";

// Ürünün hangi alanlara sahip olduğunu belirten Product tipini alıyoruz.
import { Product } from "../../types/product";

import styles from "./ProductCard.module.css";
import { useCart } from "../../context/CartContext";

// ProductCard bileşeni için gerekli olan propsları tanımlar.
type ProductCardProps = {
  product: Product;
};

// Ürünün kategorisine göre ikonunu göstermek için kullanılan fonksiyon.
function ProductVisual({
  type,
}: {
  // type değeri Product içindeki visualType alanından gelir.
  type: Product["visualType"];
}) {
  /*
    visualType değeri phone ise telefon ikonunu göstermek için
    lucide-react kütüphanesindeki Smartphone ikonunu kullanıyoruz.
  */
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
  /*
    addToCart ürünü sepete eklemek,
    increaseQuantity ürün adedini artırmak,
    decreaseQuantity ürün adedini azaltmak,
    removeFromCart ürünü tamamen kaldırmak,
    cartItems ise mevcut sepeti okumak için kullanılır.
  */
  const {
    addToCart,
    increaseQuantity,
    decreaseQuantity,
    removeFromCart,
    cartItems,
  } = useCart();

  /*
    Bu karttaki ürün daha önce sepete eklenmiş mi
    diye ürün id'sine göre arıyoruz.
  */
  const cartItem = cartItems.find(
    (item) => item.product.id === product.id
  );

  /*
    Ürün sepetteyse mevcut adedini,
    sepette değilse 0 değerini kullanıyoruz.
  */
  const cartQuantity = cartItem?.quantity ?? 0;

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

          {/*
            Ürün henüz sepette değilse normal
            "Sepete Ekle" butonu gösterilir.
          */}
          {cartQuantity === 0 ? (
            <button
              className={styles.cartButton}
              type="button"
              disabled={!product.inStock}
              onClick={() => addToCart(product)}
            >
              {product.inStock
                ? "Sepete Ekle"
                : "Tükendi"}
            </button>
          ) : (
            /*
              Ürün sepetteyse sepet butonu yerine
              adet kontrol alanı gösterilir.
            */
            <div className={styles.cartControls}>
              {/*
                Sepette yalnızca bir ürün varsa çöp kutusu
                gösterilir ve tıklandığında ürün sepetten
                tamamen kaldırılır.

                Ürün adedi birden fazlaysa eksi ikonu
                gösterilir ve ürün adedi bir azaltılır.
              */}
              {cartQuantity === 1 ? (
                <button
                  className={styles.controlButton}
                  type="button"
                  aria-label={`${product.name} ürününü sepetten kaldır`}
                  onClick={() =>
                    removeFromCart(product.id)
                  }
                >
                  <Trash2 size={19} />
                </button>
              ) : (
                <button
                  className={styles.controlButton}
                  type="button"
                  aria-label={`${product.name} adedini azalt`}
                  onClick={() =>
                    decreaseQuantity(product.id)
                  }
                >
                  <Minus size={19} />
                </button>
              )}

              {/*
                Ürünün sepette bulunan mevcut adedini
                kullanıcıya metin olarak gösterir.

                aria-live sayesinde adet değiştiğinde ekran
                okuyucuya güncel bilgi verilir.
              */}
              <span
                className={styles.quantityText}
                aria-live="polite"
              >
                {cartQuantity} ürün sepette
              </span>

              {/*
                Ürün stoktaysa artı butonu kullanılabilir.

                Tıklandığında product.id değeri
                increaseQuantity fonksiyonuna gönderilir
                ve sepetteki ürün adedi bir artırılır.
              */}
              <button
                className={styles.controlButton}
                type="button"
                aria-label={`${product.name} adedini artır`}
                disabled={!product.inStock}
                onClick={() =>
                  increaseQuantity(product.id)
                }
              >
                <Plus size={19} />
              </button>
            </div>
          )}
        </div>
      </div>
    </article>
  );
}