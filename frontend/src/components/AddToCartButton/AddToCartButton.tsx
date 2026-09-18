"use client";

import {
  Minus,
  Plus,
  ShoppingCart,
} from "lucide-react";

import Link from "next/link";

import { useCart } from "../../context/CartContext";
import type { CartProduct } from "../../types/cart";
import type { ProductDetail } from "../../types/productDetail"; 

import styles from "./AddToCartButton.module.css";

type AddToCartButtonProps = {
  product: ProductDetail;
  className: string;
  showCartLink?: boolean;
};

export default function AddToCartButton({
  product,
  className,
  showCartLink = false,
}: AddToCartButtonProps) {
  const cartProduct: CartProduct = {
    id: product.id, 
    name: product.name,
    model: product.model,
    brand: product.brand,
    category: product.category,
    price: product.price,
    vatRate: product.vatRate,
    image: product.images[0] ?? null, 
    inStock: product.inStock,
    stockQuantity: product.stock, 
  };

  const {
    addToCart,
    cartItems,
    increaseQuantity,
    decreaseQuantity,
    removeFromCart,
  } = useCart();

  const cartItem = cartItems.find(
    (item) => item.product.id === cartProduct.id
  );

  const cartQuantity = cartItem?.quantity ?? 0;
  const displayedQuantity = cartQuantity;

  function decreaseSelectedQuantity() {
    if (cartQuantity === 0) {
      return;
    }

    if (cartQuantity === 1) {
      removeFromCart(cartProduct.id);
      return;
    }

    decreaseQuantity(cartProduct.id);
  }

  function increaseSelectedQuantity() {
    if (cartQuantity === 0) {
      addToCart(cartProduct, 1);
      return;
    }

    increaseQuantity(cartProduct.id);
  }

  function handleAddToCart() {
    addToCart(cartProduct, 1);
  }

  const hasReachedStockLimit = cartQuantity >= product.stock;

  const isDecreaseDisabled = cartQuantity === 0;

  const isAddButtonDisabled =
    !product.inStock ||
    hasReachedStockLimit;

  return (
    <div className={styles.purchaseArea}>
      {product.inStock && (
        <div
          className={styles.quantityControl}
          aria-label="Ürün adedi"
        >
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

          <span
            className={styles.quantityValue}
            aria-live="polite"
          >
            {displayedQuantity}
          </span>

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

      <button
        className={`${className} ${styles.cartButtonWithBadge}`}
        type="button"
        disabled={isAddButtonDisabled}
        onClick={handleAddToCart}
      >
        {product.inStock
          ? "Sepete Ekle"
          : "Ürün Tükendi"}

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