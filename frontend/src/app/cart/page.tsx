"use client";

import {
  ArrowLeft,
  Minus,
  Plus,
  ShoppingBag,
  Trash2,
} from "lucide-react";

import Link from "next/link";

import { useCart } from "../../context/CartContext";
import styles from "./page.module.css";

export default function CartPage() {
  
  // ilgili sepet bilgilerini aliriz
  const {
    cartItems,
    increaseQuantity,
    decreaseQuantity,
    removeFromCart,
    clearCart,
    totalQuantity,
    totalPrice,
  } = useCart();

  return (
    <main className={styles.cartPage}>
      <header className={styles.topBar}>
        <Link className={styles.logo} href="/">
          TECHCART
        </Link>

        <Link
          className={styles.backLink}
          href="/#products"
        >
          <ArrowLeft size={18} />
          Alışverişe devam et
        </Link>
      </header>

      <section className={styles.cartContainer}>
        <div className={styles.cartHeader}>
          <div>
            <span className={styles.label}>
              ALIŞVERİŞ SEPETİ
            </span>

            <h1>Sepetim</h1>

            <p>
              Sepetinde toplam {totalQuantity} ürün bulunuyor.
            </p>
          </div>

          {/*
            Sepette ürün varsa temizleme butonunu gösterir.
          */}
          {cartItems.length > 0 && (
            <button
              className={styles.clearButton}
              type="button"
              onClick={clearCart}
            >
              <Trash2 size={17} />
              Sepeti temizle
            </button>
          )}
        </div>

        {/*
          Sepet boşsa kullanıcıya boş sepet görünümü gösterilir.
        */}
        {cartItems.length === 0 ? (
          <div className={styles.emptyCart}>
            <div className={styles.emptyIcon}>
              <ShoppingBag
                size={52}
                strokeWidth={1.3}
              />
            </div>

            <h2>Sepetin henüz boş</h2>

            <p>
              Ürünleri inceleyerek beğendiğin teknoloji
              ürünlerini sepetine ekleyebilirsin.
            </p>

            <Link
              className={styles.discoverButton}
              href="/#products"
            >
              Ürünleri Keşfet
            </Link>
          </div>
        ) : (
          /*
            Sepette ürün varsa ürün listesiyle
            sipariş özetini yan yana gösterir.
          */
          <div className={styles.cartContent}>
            <div className={styles.cartItems}>
              {cartItems.map((item) => (
                <article
                  className={styles.cartItem}
                  key={item.product.id}
                >
                  <div className={styles.productVisual}>
                    <ShoppingBag
                      size={42}
                      strokeWidth={1.3}
                    />
                  </div>

                  <div className={styles.productInformation}>
                    <span className={styles.productMeta}>
                      {item.product.brand}
                      {" • "}
                      {item.product.category}
                    </span>

                    <Link
                      href={`/products/${item.product.id}`}
                    >
                      {item.product.name}
                    </Link>

                    <span>
                      Model: {item.product.model}
                    </span>
                  </div>

                  {/*
                    Eksi, mevcut adet ve artı butonlarını
                    bir arada tutan miktar kontrolü.
                  */}
                  <div className={styles.quantityControl}>
                    <button
                      type="button"
                      aria-label="Ürün adedini azalt"
                      disabled={item.quantity === 1}
                      onClick={() =>
                        decreaseQuantity(item.product.id)
                      }
                    >
                      <Minus size={16} />
                    </button>

                    <span>{item.quantity}</span>

                    <button
                      type="button"
                      aria-label="Ürün adedini artır"
                      onClick={() =>
                        increaseQuantity(item.product.id)
                      }
                    >
                      <Plus size={16} />
                    </button>
                  </div>

                  <div className={styles.itemPrice}>
                    <strong>
                      {(
                        item.product.price * item.quantity
                      ).toLocaleString("tr-TR")}{" "}
                      ₺
                    </strong>

                    {item.quantity > 1 && (
                      <span>
                        Birim fiyat:{" "}
                        {item.product.price.toLocaleString(
                          "tr-TR"
                        )}{" "}
                        ₺
                      </span>
                    )}
                  </div>

                  <button
                    className={styles.removeButton}
                    type="button"
                    aria-label="Ürünü sepetten kaldır"
                    onClick={() =>
                      removeFromCart(item.product.id)
                    }
                  >
                    <Trash2 size={19} />
                  </button>
                </article>
              ))}
            </div>

            <aside className={styles.orderSummary}>
              <h2>Sipariş Özeti</h2>

              <div className={styles.summaryRow}>
                <span>Ürün adedi</span>
                <strong>{totalQuantity}</strong>
              </div>

              <div className={styles.summaryRow}>
                <span>Ara toplam</span>

                <strong>
                  {totalPrice.toLocaleString("tr-TR")} ₺
                </strong>
              </div>

              <div className={styles.summaryRow}>
                <span>Kargo</span>
                <strong>Ücretsiz</strong>
              </div>

              <div className={styles.totalRow}>
                <span>Toplam</span>

                <strong>
                  {totalPrice.toLocaleString("tr-TR")} ₺
                </strong>
              </div>

              {/*
                Ödeme ekranını daha sonra oluşturacağız.
                Şimdilik /checkout adresine yönlendirir.
              */}
              <Link
                className={styles.checkoutButton}
                href="/checkout"
              >
                Ödemeye Geç
              </Link>

              <p className={styles.summaryInformation}>
                Ödeme işlemine geçtiğinde teslimat ve
                ödeme bilgilerini girebilirsin.
              </p>
            </aside>
          </div>
        )}
      </section>
    </main>
  );
}