"use client";

import {
  Minus,
  Plus,
} from "lucide-react";

import { useState } from "react";

import { useCart } from "../../context/CartContext";
import { Product } from "../../types/product";

import styles from "./AddToCartButton.module.css";

// Detay sayfasındaki sepete ekleme işlemi için gerekli propslar.
type AddToCartButtonProps = {
  product: Product;
  className: string;
};

export default function AddToCartButton({
  product,
  className,
}: AddToCartButtonProps) {
  /*
    addToCart, seçilen ürünü ve adedi sepete eklemek için;

    cartItems ise ürünün sepetteki güncel adedini
    bulmak için kullanılır.
  */
  const {
    addToCart,
    cartItems,
  } = useCart();

  /*
    quantity, kullanıcının ürün detay ekranında
    sepete eklemek üzere seçtiği adedi tutar.
  */
  const [quantity, setQuantity] = useState(1);

  /*
    Bu ürünün sepette daha önce bulunup
    bulunmadığını ürün id'sine göre kontrol ederiz.
  */
  const cartItem = cartItems.find(
    (item) => item.product.id === product.id
  );

  /*
    Ürün sepetteyse güncel adedini,
    sepette değilse sıfır değerini kullanırız.
  */
  const cartQuantity = cartItem?.quantity ?? 0;

  /*
    Kullanıcı eksi butonuna bastığında seçilen
    adedi bir azaltır.

    Math.max sayesinde adet birin altına düşmez.
  */
  function decreaseSelectedQuantity() {
    setQuantity((currentQuantity) =>
      Math.max(1, currentQuantity - 1)
    );
  }

  /*
    Kullanıcı artı butonuna bastığında seçilen
    adedi bir artırır.

    Stok miktarı tanımlanmışsa seçilen adet
    stok miktarını geçemez.
  */
  function increaseSelectedQuantity() {
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
    Sepete Ekle butonuna basıldığında ürünle
    birlikte kullanıcının seçtiği adet sepete gönderilir.
  */
  function handleAddToCart() {
    addToCart(product, quantity);
  }

  /*
    Stok miktarı tanımlanmışsa kullanıcının
    en yüksek adede ulaşıp ulaşmadığını kontrol eder.
  */
  const hasReachedStockLimit =
    product.stockQuantity !== undefined &&
    quantity >= product.stockQuantity;

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
            aria-label="Ürün adedini azalt"
            disabled={quantity === 1}
            onClick={decreaseSelectedQuantity}
          >
            <Minus size={18} />
          </button>

          {/* Kullanıcının seçtiği mevcut ürün adedi */}
          <span
            className={styles.quantityValue}
            aria-live="polite"
          >
            {quantity}
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
        disabled={!product.inStock}
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
    </div>
  );
}