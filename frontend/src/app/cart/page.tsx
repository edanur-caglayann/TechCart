"use client";

import {
  ArrowLeft,
  Minus,
  Plus,
  ShoppingBag,
  Trash2,
} from "lucide-react";

import Link from "next/link";
import { useRouter } from "next/navigation";

import { useAuth } from "../../context/AuthContext";
import { useCart } from "../../context/CartContext";

import {
  calculateIncludedVat,
  calculateNetAmount,
  DEFAULT_VAT_RATE,
  formatCurrency,
} from "../../utils/tax";

import styles from "./page.module.css";

export default function CartPage() {
  // Sayfalar arasında yönlendirme yapmak için kullanılır.
  const router = useRouter();

  /*
    Kullanıcının giriş durumunu ve localStorage kontrolünün tamamlanıp tamamlanmadığını alırız.
  */
  const {
    isAuthenticated,
    isAuthLoading,
  } = useAuth();

  // İlgili sepet bilgilerini alırız.
  const {
    cartItems,
    increaseQuantity,
    decreaseQuantity,
    removeFromCart,
    clearCart,
    totalQuantity,
    totalPrice,
  } = useCart();

  /*
    Sepetteki her ürünün KDV hariç tutarınınhesaplayarak genel KDV hariç toplamı buluruz
  */
  const totalNetPrice = cartItems.reduce(
    (total, item) => {
      const itemTotal =
        item.product.price * item.quantity;

      const vatRate =
        item.product.vatRate ?? DEFAULT_VAT_RATE;

      return (
        total +
        calculateNetAmount(itemTotal, vatRate)
      );
    },
    0
  );

  /*
    Sepetteki ürünlerin fiyatlarına dahil olan KDV tutarlarını toplayarak toplam KDV'yi hespalr
  */
  const totalVat = cartItems.reduce(
    (total, item) => {
      const itemTotal =
        item.product.price * item.quantity;

      const vatRate =
        item.product.vatRate ?? DEFAULT_VAT_RATE;

      return (
        total +
        calculateIncludedVat(itemTotal, vatRate)
      );
    },
    0
  );

  /*
    Ödemeye Geç butonuna basıldığında kullanıcının giriş yapıp yapmadığını kontrol eder.
    Kullanıcı giriş yapmadıysa giriş ekranına, giriş yaptıysa ödeme ekranına yönlendirilir.
  */
  function handleCheckout() {
    // Oturum kontrolü devam ediyorsa işlem yapılmaz.
    if (isAuthLoading) {
      return;
    }

    if (!isAuthenticated) {
      /*
        kullanıcının sepetine geri dönmesi icin -> returnUrl
      */
      router.push("/login?returnUrl=/cart");
      return;
    }

    router.push("/checkout");
  }

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
              Sepetinde toplam {totalQuantity} ürün
              bulunuyor.
            </p>
          </div>

          {/* Sepette ürün varsa temizleme butonunu gösterir. */}
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

        {/* Sepet boşsa boş sepet görünümü gösterilir. */}
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
          // Sepette ürün varsa ürün listesiyle sipariş özetini yan yana gösterir.
          <div className={styles.cartContent}>
            <div className={styles.cartItems}>
              {cartItems.map((item) => {
                // Ürünün adedine göre KDV dahil toplamı.
                const itemTotal =
                  item.product.price * item.quantity;

                const vatRate =
                  item.product.vatRate ??
                  DEFAULT_VAT_RATE;

                // Ürün toplamının içindeki KDV tutarı.
                const itemVat =
                  calculateIncludedVat(
                    itemTotal,
                    vatRate
                  );

                return (
                  <article
                    className={styles.cartItem}
                    key={item.product.id}
                  >
                    <div
                      className={styles.productVisual}
                    >
                      <ShoppingBag
                        size={42}
                        strokeWidth={1.3}
                      />
                    </div>

                    <div
                      className={
                        styles.productInformation
                      }
                    >
                      <span
                        className={styles.productMeta}
                      >
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
                      Eksi, mevcut adet ve artı
                      butonlarını bir arada tutar.
                    */}
                    <div
                      className={styles.quantityControl}
                    >
                      <button
                        type="button"
                        aria-label="Ürün adedini azalt"
                        disabled={item.quantity === 1}
                        onClick={() =>
                          decreaseQuantity(
                            item.product.id
                          )
                        }
                      >
                        <Minus size={16} />
                      </button>

                      <span>{item.quantity}</span>

                      <button
                        type="button"
                        aria-label="Ürün adedini artır"
                        onClick={() =>
                          increaseQuantity(
                            item.product.id
                          )
                        }
                      >
                        <Plus size={16} />
                      </button>
                    </div>

                    <div className={styles.itemPrice}>
                      <strong>
                        {formatCurrency(itemTotal)}
                      </strong>

                      <span>
                        %{vatRate} KDV dahil
                      </span>

                      <span>
                        Fiyata dahil KDV:{" "}
                        {formatCurrency(itemVat)}
                      </span>

                      {item.quantity > 1 && (
                        <span>
                          Birim fiyat:{" "}
                          {formatCurrency(
                            item.product.price
                          )}
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
                );
              })}
            </div>

            <aside className={styles.orderSummary}>
              <h2>Sipariş Özeti</h2>

              <div className={styles.summaryRow}>
                <span>Ürün adedi</span>
                <strong>{totalQuantity}</strong>
              </div>

              <div className={styles.summaryRow}>
                <span>KDV hariç ara toplam</span>

                <strong>
                  {formatCurrency(totalNetPrice)}
                </strong>
              </div>

              <div className={styles.summaryRow}>
                <span>Toplam KDV</span>

                <strong>
                  {formatCurrency(totalVat)}
                </strong>
              </div>

              <div className={styles.summaryRow}>
                <span>Kargo</span>
                <strong>Ücretsiz</strong>
              </div>

              <div className={styles.totalRow}>
                <span>KDV dahil toplam</span>

                <strong>
                  {formatCurrency(totalPrice)}
                </strong>
              </div>

              {/*
                Ödeme öncesinde kullanıcının giriş durumu handleCheckout ile kontrol edilir.
              */}
              <button
                className={styles.checkoutButton}
                type="button"
                disabled={isAuthLoading}
                onClick={handleCheckout}
              >
                {isAuthLoading
                  ? "Oturum kontrol ediliyor..."
                  : "Ödemeye Geç"}
              </button>

              <p
                className={styles.summaryInformation}
              >
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