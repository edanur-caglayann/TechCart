"use client";

import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";

import type { CartItem } from "../types/cart";
import type { CartProduct } from "../types/cart";

// Backend'in liste response'u sayısal bir stok adedi (stockQuantity) vermiyor,
// sadece inStock (true/false) veriyor — gerçek stok sayısı sadece detay
// endpoint'inde var. O yüzden sepette adet artırırken sınırsız değil ama
// gerçek stoğa da bağlı olmayan, makul bir güvenlik sınırı kullanıyoruz.
// GERÇEK stok doğrulaması Sepet Yönetimi'nde backend'e bağlanınca gelecek.
const FALLBACK_MAX_QUANTITY = 99;

type CartContextType = {
  cartItems: CartItem[];
  addToCart: (product: CartProduct, quantity?: number) => void;
  increaseQuantity: (productId: string) => void; // number -> string
  decreaseQuantity: (productId: string) => void; // number -> string
  removeFromCart: (productId: string) => void;   // number -> string
  clearCart: () => void;
  totalQuantity: number;
  totalPrice: number;
};

const CartContext = createContext<CartContextType | undefined>(undefined);

type CartProviderProps = {
  children: ReactNode;
};

export function CartProvider({ children }: CartProviderProps) {
  const [cartItems, setCartItems] = useState<CartItem[]>([]);

  useEffect(() => {
    const savedCart = localStorage.getItem("techcart-cart");

    if (savedCart) {
      try {
        const parsedCart: CartItem[] = JSON.parse(savedCart);
        setCartItems(parsedCart);
      } catch {
        localStorage.removeItem("techcart-cart");
      }
    }
  }, []);

  useEffect(() => {
    localStorage.setItem("techcart-cart", JSON.stringify(cartItems));
  }, [cartItems]);

  function addToCart(product: CartProduct, quantity = 1) {
    setCartItems((currentItems) => {
      const quantityToAdd = Math.max(1, quantity);

      const existingItem = currentItems.find((item) => item.product.id === product.id);

      if (existingItem) {
        return currentItems.map((item) =>
          item.product.id === product.id
            ? { ...item, quantity: Math.min(item.quantity + quantityToAdd, FALLBACK_MAX_QUANTITY) }
            : item
        );
      }

      return [...currentItems, { product, quantity: Math.min(quantityToAdd, FALLBACK_MAX_QUANTITY) }];
    });
  }

  function increaseQuantity(productId: string) {
    setCartItems((currentItems) =>
      currentItems.map((item) =>
        item.product.id === productId
          ? { ...item, quantity: Math.min(item.quantity + 1, FALLBACK_MAX_QUANTITY) }
          : item
      )
    );
  }

  function decreaseQuantity(productId: string) {
    setCartItems((currentItems) =>
      currentItems.map((item) =>
        item.product.id === productId ? { ...item, quantity: Math.max(1, item.quantity - 1) } : item
      )
    );
  }

  function removeFromCart(productId: string) {
    setCartItems((currentItems) => currentItems.filter((item) => item.product.id !== productId));
  }

  function clearCart() {
    setCartItems([]);
  }

  const totalQuantity = cartItems.reduce((total, item) => total + item.quantity, 0);
  const totalPrice = cartItems.reduce((total, item) => total + item.product.price * item.quantity, 0);

  return (
    <CartContext.Provider
      value={{ cartItems, addToCart, increaseQuantity, decreaseQuantity, removeFromCart, clearCart, totalQuantity, totalPrice }}
    >
      {children}
    </CartContext.Provider>
  );
}

export function useCart() {
  const context = useContext(CartContext);

  if (!context) {
    throw new Error("useCart, CartProvider içerisinde kullanılmalıdır.");
  }

  return context;
}
