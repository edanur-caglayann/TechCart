"use client";

import {
  createContext,
  useContext,
  useEffect,
  useRef,
  useState,
  type ReactNode,
} from "react";

import type { CartItem, CartProduct } from "../types/cart";
import type { CartLineResponse, CartResponse } from "../types/cartResponse";
import { useAuth } from "./AuthContext";
import {
  addCartItemRequest,
  clearCartRequest,
  mergeCartRequest,
  removeCartItemRequest,
  updateCartItemQuantityRequest,
} from "../services/cartService";

const FALLBACK_MAX_QUANTITY = 99;
const STORAGE_KEY = "techcart-cart";

function toCartItems(response: CartResponse): CartItem[] {
  return response.items.map((line: CartLineResponse) => ({
    product: {
      id: line.productId,
      name: line.name,
      model: line.model,
      brand: line.brand,
      category: line.category,
      price: line.unitPrice,
      vatRate: line.vatRate,
      image: line.image,
      inStock: line.inStock,
      stockQuantity: line.stock,
    },
    quantity: line.quantity,
  }));
}

type CartContextType = {
  cartItems: CartItem[];
  addToCart: (product: CartProduct, quantity?: number) => void;
  increaseQuantity: (productId: string) => void;
  decreaseQuantity: (productId: string) => void;
  removeFromCart: (productId: string) => void;
  clearCart: () => void;
  totalQuantity: number;
  totalPrice: number;
};

const CartContext = createContext<CartContextType | undefined>(undefined);

type CartProviderProps = {
  children: ReactNode;
};

export function CartProvider({ children }: CartProviderProps) {
  const { isAuthenticated, isAuthLoading } = useAuth();
  const [cartItems, setCartItems] = useState<CartItem[]>([]);
  const hasMergedRef = useRef(false);

  useEffect(() => {
    if (isAuthLoading || isAuthenticated) return;

    const savedCart = localStorage.getItem(STORAGE_KEY);

    if (savedCart) {
      try {
        setCartItems(JSON.parse(savedCart));
      } catch {
        localStorage.removeItem(STORAGE_KEY);
      }
    }
  }, [isAuthenticated, isAuthLoading]);

  useEffect(() => {
    if (isAuthLoading || isAuthenticated) return;
    localStorage.setItem(STORAGE_KEY, JSON.stringify(cartItems));
  }, [cartItems, isAuthenticated, isAuthLoading]);

  useEffect(() => {
    if (isAuthLoading) return;

    if (!isAuthenticated) {
      hasMergedRef.current = false;
      return;
    }

    if (hasMergedRef.current) return;
    hasMergedRef.current = true;

    const savedCart = localStorage.getItem(STORAGE_KEY);
    let localItems: CartItem[] = [];

    if (savedCart) {
      try {
        localItems = JSON.parse(savedCart);
      } catch {
        localItems = [];
      }
    }

    const payload = localItems.map((item) => ({
      productId: item.product.id,
      quantity: item.quantity,
    }));

    mergeCartRequest(payload).then((response) => {
      setCartItems(toCartItems(response));
      localStorage.removeItem(STORAGE_KEY);
    });
  }, [isAuthenticated, isAuthLoading]);

  function addToCart(product: CartProduct, quantity = 1) {
    if (isAuthenticated) {
      addCartItemRequest(product.id, quantity).then((response) => {
        setCartItems(toCartItems(response));
      });
      return;
    }

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
    if (isAuthenticated) {
      const current = cartItems.find((item) => item.product.id === productId);
      const newQuantity = (current?.quantity ?? 0) + 1;

      updateCartItemQuantityRequest(productId, newQuantity).then((response) => {
        setCartItems(toCartItems(response));
      });
      return;
    }

    setCartItems((currentItems) =>
      currentItems.map((item) =>
        item.product.id === productId
          ? { ...item, quantity: Math.min(item.quantity + 1, FALLBACK_MAX_QUANTITY) }
          : item
      )
    );
  }

  function decreaseQuantity(productId: string) {
    if (isAuthenticated) {
      const current = cartItems.find((item) => item.product.id === productId);
      if (!current || current.quantity <= 1) return;

      updateCartItemQuantityRequest(productId, current.quantity - 1).then((response) => {
        setCartItems(toCartItems(response));
      });
      return;
    }

    setCartItems((currentItems) =>
      currentItems.map((item) =>
        item.product.id === productId ? { ...item, quantity: Math.max(1, item.quantity - 1) } : item
      )
    );
  }

  function removeFromCart(productId: string) {
    if (isAuthenticated) {
      removeCartItemRequest(productId).then((response) => {
        setCartItems(toCartItems(response));
      });
      return;
    }

    setCartItems((currentItems) => currentItems.filter((item) => item.product.id !== productId));
  }

  function clearCart() {
    if (isAuthenticated) {
      clearCartRequest().then(() => {
        setCartItems([]);
      });
      return;
    }

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