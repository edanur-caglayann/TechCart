"use client";

import {
  Minus,
  Plus,
  ShoppingCart,
} from "lucide-react";

import { useState } from "react";

import { useCart } from "../../context/CartContext";

import { Product } from "../../types/product";

import Link from "next/link";

import styles from "./AddToCartButton.module.css";

// Detay sayfasındaki sepete ekleme işlemi için gerekli propslar.
type AddToCartButtonProps = {
  product: Product;
  className: string;

  // Sepete Git bağlantısının gösterilip gösterilmeyeceği.
  showCartLink?: boolean;
};

export default function AddToCartButton({
  product,
  className,
  showCartLink = false,
}: AddToCartButtonProps) {
  /*
    addToCart, seçilen ürünü ve adedi sepete eklemek için;
    cartItems ise ürünün sepetteki güncel adedini bulmak için kullanılır.
  */
  const {
    addToCart,
    cartItems,
    increaseQuantity,
    decreaseQuantity,
    removeFromCart,
  } = useCart();

  /*
    quantity, kullanıcının ürün detay ekranında sepete eklemek üzere seçtiği adedi tutar.
  */
  const [quantity, setQuantity] = useState(1);

  /*
    Bu ürünün sepette daha önce bulunup bulunmadığını ürün id'sine göre kontrol ederiz.
  */
  const cartItem = cartItems.find(
    (item) => item.product.id === product.id
  );

  /*
    Ürün sepetteyse güncel adedini, sepette değilse sıfır değerini kullanırız.
  */
  const cartQuantity = cartItem?.quantity ?? 0;

  /*
    Ürün sepetteyse sepet içerisindeki gerçek adedi, sepette değilse kullanıcının seçtiği adedi gösteririz.
  */
  const displayedQuantity =
    cartQuantity > 0 ? cartQuantity : quantity;

  /*
    Ürün sepetteyse sepet adedi azaltılır. 
    Sepette yalnızca bir adet ürün varsa eksi butonuna basıldığında ürün sepetten kaldırılır.
  */
  function decreaseSelectedQuantity() {
    if (cartQuantity > 0) {
      if (cartQuantity === 1) {
        removeFromCart(product.id);
        return;
      }

      decreaseQuantity(product.id);
      return;
    }

    setQuantity((currentQuantity) =>
      Math.max(1, currentQuantity - 1)
    );
  }

  /*
    Kullanıcı artı butonuna bastığında seçilen adedi bir artırır.
    Stok miktarı tanımlanmışsa seçilen adet stok miktarını geçemez.
  */

  /*
    Ürün sepetteyse artı butonu sepet içerisindeki gerçek ürün adedini artırır.
  */
  function increaseSelectedQuantity() {
    if (cartQuantity > 0) {
      increaseQuantity(product.id);
      return;
    }

    setQuantity((currentQuantity) => {
      if (
        product.stockQuantity !== undefined &&
        currentQuantity >= product.stockQuantity
      ) {
        return currentQuantity;
      }

      return currentQuantity + 1;
    });
  }

  /*
    Sepete Ekle butonuna basıldığında ürünle birlikte kullanıcının seçtiği adet sepete gönderilir.
  */
  function handleAddToCart() {
    /*
      Ürün zaten sepetteyse butona her basıldığında sepete bir adet daha eklenir.
    */
    if (cartQuantity > 0) {
      addToCart(product, 1);
      return;
    }

    addToCart(product, quantity);

    /*
      İlk ekleme tamamlandıktan sonra geçici adet seçimini tekrar bire döndürürüz.
    */
    setQuantity(1);
  }

  /*
    Stok miktarı tanımlanmışsa kullanıcının en yüksek adede ulaşıp ulaşmadığını kontrol eder.
  */
  const hasReachedStockLimit =
    product.stockQuantity !== undefined &&
    displayedQuantity >= product.stockQuantity;

  /*
    Ürün henüz sepette değilse seçilen adet

    birin altına düşemeyeceği için eksi butonu

    bir değerinde devre dışı bırakılır.

    Ürün sepetteyse bir değerinde aktif kalır ve

    ürünü sepetten kaldırır.
  */
  const isDecreaseDisabled =
    cartQuantity === 0 && quantity === 1;

  /*
    Ürün sepetteyken stok sınırına ulaşıldıysa

    Sepete Ekle butonuyla daha fazla ürün eklenemez.
  */
  const isAddButtonDisabled =
    !product.inStock ||
    (cartQuantity > 0 && hasReachedStockLimit);

  return (
    <div className={styles.purchaseArea}>
      {/*
        Stoktaki ürünler için adet azaltma,

        mevcut adet ve adet artırma alanı gösterilir.
      */}
      {product.inStock && (
        <div
          className={styles.quantityControl}
          aria-label="Ürün adedi"
        >
          {/* Seçilen adedi bir azaltır */}
          <button
            className={styles.quantityButton}
            type="button"
            aria-label={
              cartQuantity === 1
                ? "Ürünü sepetten kaldır"
                : "Ürün adedini azalt"
            }
            disabled={isDecreaseDisabled}
            onClick={decreaseSelectedQuantity}
          >
            <Minus size={18} />
          </button>

          {/* Kullanıcının seçtiği mevcut ürün adedi */}
          <span
            className={styles.quantityValue}
            aria-live="polite"
          >
            {displayedQuantity}
          </span>

          {/* Seçilen adedi stok sınırına kadar artırır */}
          <button
            className={styles.quantityButton}
            type="button"
            aria-label="Ürün adedini artır"
            disabled={hasReachedStockLimit}
            onClick={increaseSelectedQuantity}
          >
            <Plus size={18} />
          </button>
        </div>
      )}

      {/*
        Ürün stoktaysa seçilen adet sepete eklenir.

        Ürün stokta değilse buton devre dışı kalır

        ve kullanıcıya "Ürün Tükendi" metni gösterilir.
      */}
      <button
        className={`${className} ${styles.cartButtonWithBadge}`}
        type="button"
        disabled={isAddButtonDisabled}
        onClick={handleAddToCart}
      >
        {product.inStock
          ? "Sepete Ekle"
          : "Ürün Tükendi"}

        {/*
          Ürün sepete en az bir kez eklendiyse

          butonun sağ üst köşesinde sepetteki

          güncel ürün adedini gösterir.
        */}
        {cartQuantity > 0 && (
          <span
            className={styles.cartBadge}
            aria-label={`Sepette ${cartQuantity} adet var`}
            aria-live="polite"
          >
            {cartQuantity}
          </span>
        )}
      </button>

      {/*
        Bu bileşen ürün detayında kullanılıyorsa ve

        ürün sepette bulunuyorsa Sepete Git butonu çıksın.
      */}
      {showCartLink && cartQuantity > 0 && (
        <Link
          className={styles.goToCartButton}
          href="/cart"
        >
          <ShoppingCart size={18} />
          Sepete Git
        </Link>
      )}
    </div>
  );
}