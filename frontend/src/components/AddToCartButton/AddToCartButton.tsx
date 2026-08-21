"use client";

import { useCart } from "../../context/CartContext";
import { Product } from "../../types/product";

// detay sayfasindaki sepete ekleme islemi icin
type AddToCartButtonProps = {
  product: Product;

  className: string;
};

export default function AddToCartButton({
  product,
  className,
}: AddToCartButtonProps) {
  const { addToCart } = useCart();

  return (
    <button
      className={className}
      type="button"
      disabled={!product.inStock}
      onClick={() => addToCart(product)}
    >
      {product.inStock
        ? "Sepete Ekle"
        : "Ürün Tükendi"}
    </button>
  );
}