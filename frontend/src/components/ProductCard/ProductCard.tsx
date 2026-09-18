"use client";

import { Camera, Cpu, Disc3, Headphones, Keyboard, Laptop, Minus, Mouse, Plus, Smartphone, Speaker, Tablet, Trash2, Tv, Watch } from "lucide-react";
import Image from "next/image";
import Link from "next/link";
import { useRouter } from "next/navigation"; 

import type { ProductListItem } from "../../types/productListItem";
import styles from "./ProductCard.module.css";
import { useCart } from "../../context/CartContext";

type ProductCardProps = { product: ProductListItem };

function ProductFallbackVisual({ category }: { category: string }) {
  switch (category) {
    case "Bilgisayar": return <Laptop size={120} strokeWidth={1.2} />;
    case "Tablet": return <Tablet size={100} strokeWidth={1.2} />;
    case "Klavye": return <Keyboard size={100} strokeWidth={1.2} />;
    case "Mouse": return <Mouse size={100} strokeWidth={1.2} />;
    case "Pikap": return <Disc3 size={105} strokeWidth={1.2} />;
    case "Kulaklık": return <Headphones size={105} strokeWidth={1.2} />;
    case "Hoparlör": return <Speaker size={105} strokeWidth={1.2} />;
    case "Televizyon": return <Tv size={120} strokeWidth={1.2} />;
    case "Akıllı Saat": return <Watch size={100} strokeWidth={1.2} />;
    case "Kamera": return <Camera size={105} strokeWidth={1.2} />;
    case "Telefon": return <Smartphone size={95} strokeWidth={1.2} />;
    default: return <Cpu size={105} strokeWidth={1.2} />;
  }
}

export default function ProductCard({ product }: ProductCardProps) {
  const router = useRouter();
  const { addToCart, increaseQuantity, decreaseQuantity, removeFromCart, cartItems } = useCart();

  const cartItem = cartItems.find((item) => item.product.id === product.id);
  const cartQuantity = cartItem?.quantity ?? 0;

  function goToDetail() {
    router.push(`/products/${product.id}`);
  }

  return (
    <article
      className={styles.card}
      onClick={goToDetail}
      role="link"
      tabIndex={0}
      onKeyDown={(event) => {
        if (event.key === "Enter") goToDetail();
      }}
    >
      <div className={styles.visual}>
        {product.image ? (
          <Image src={product.image} alt={`${product.name} ürün görseli`} fill sizes="(max-width: 600px) 50vw, 25vw" className={styles.productImage} />
        ) : (
          <ProductFallbackVisual category={product.category} />
        )}
        {!product.inStock && <span className={styles.outOfStockBadge}>Tükendi</span>}
      </div>

      <div className={styles.content}>
        <div className={styles.productMeta}>
          <span className={styles.brand}>{product.brand}</span>
          <span className={styles.separator}>•</span>
          <span className={styles.category}>{product.category}</span>
        </div>

        <h3 className={styles.productName}>{product.name}</h3>

        <strong className={styles.price}>{product.price.toLocaleString("tr-TR")} ₺</strong>

        <div className={styles.actions} onClick={(event) => event.stopPropagation()}>
          <Link className={styles.detailsButton} href={`/products/${product.id}`}>Detayları Gör</Link>

          {cartQuantity === 0 ? (
            <button className={styles.cartButton} type="button" disabled={!product.inStock} onClick={() => addToCart(product)}>
              {product.inStock ? "Sepete Ekle" : "Tükendi"}
            </button>
          ) : (
            <div className={styles.cartControls}>
              {cartQuantity === 1 ? (
                <button className={styles.controlButton} type="button" aria-label={`${product.name} ürününü sepetten kaldır`} onClick={() => removeFromCart(product.id)}><Trash2 size={19} /></button>
              ) : (
                <button className={styles.controlButton} type="button" aria-label={`${product.name} adedini azalt`} onClick={() => decreaseQuantity(product.id)}><Minus size={19} /></button>
              )}
              <span className={styles.quantityText} aria-live="polite">{cartQuantity} ürün sepette</span>
              <button className={styles.controlButton} type="button" aria-label={`${product.name} adedini artır`} disabled={!product.inStock} onClick={() => increaseQuantity(product.id)}><Plus size={19} /></button>
            </div>
          )}
        </div>
      </div>
    </article>
  );
}