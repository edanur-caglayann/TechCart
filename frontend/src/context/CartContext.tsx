
/* Bu dosyada useState, useEffect ve localStorage kullanilir. 
Bunlar tarayici tarafinda calistigi icin dosyayi Client compoent
oalrak isaretleriz
*/
"use client";

import {
    createContext,
  ReactNode,
  useContext,
  useEffect,
  useState,
} from "react";

import { CartItem } from "../types/cart";
import { Product } from "../types/product";

// Yapilabilecek sepet islemlerinin tiplerini belirleriz
type CartContextType = {
    cartItems: CartItem[];

    addToCart: (product: Product) => void;
    increaseQuantity: (product: number) => void; // miktari arttir
    decreaseQuantity: (product: number) => void; // miktari azlt
    removeFromCart: (product: number) => void;
    clearCart: () => void;

    totalQuantity: number;
    totalPrice: number;
};

const CartContext = createContext<
  CartContextType | undefined
>(undefined);

type CartProviderProps = {
  children: ReactNode;
};

export function CartProvider({
    children,
}:CartProviderProps) {

    /* cartItems -> sepetin mevcut durumu
    setCartItems -> sepeti degistirmek icin kullandigimiz fonks
    */
    const [cartItems, setCartItems] = useState<CartItem[]> (
        []
    );

    // sayfa ilk acildiginda localStorage icindeki
    // onceden kaydedilen sepeti okuyarak cartItems state'ine aktarir
    useEffect(() => {
        const savedCart = localStorage.getItem(
      "techcart-cart"
    );

    /* localStorage verileri metin olarak sakladigi icin
    metni tekrar js dizisine donustururuz
    */
     if(savedCart) {
       try {
        const parsedCart: CartItem[] =
          JSON.parse(savedCart);

        setCartItems(parsedCart);
      } catch {
        /*
          Kaydedilen veri bozuksa bozuk sepet veriis silinir
        */
        localStorage.removeItem("techcart-cart");
      }
    }
  }, []);
    
// cartItems degistiginde guncel sepeti localStorage icine kaydeder
useEffect(() => {
    localStorage.setItem(
      "techcart-cart",
      JSON.stringify(cartItems)
    );
  }, [cartItems]);

  /*
    Sepete ürün ekleme işlemi.

    Ürün zaten sepetteyse yeni satır oluşturmak yerine
    mevcut ürünün adedini bir artırır.
  */
  function addToCart(product: Product) {
    setCartItems((currentItems) => {
      const existingItem = currentItems.find(
        (item) => item.product.id === product.id
      );

      if (existingItem) {
        return currentItems.map((item) =>
          item.product.id === product.id
            ? {
                ...item,
                quantity: item.quantity + 1,
              }
            : item
        );
      }

      /*
        Ürün sepette yoksa bir adet olacak şekilde
        yeni sepet elemanı oluşturur.
      */
      return [
        ...currentItems,
        {
          product,
          quantity: 1,
        },
      ];
    });
  }

  /* arti butonuna basilinca urün adedini bir artırır. */
  function increaseQuantity(productId: number) {
    setCartItems((currentItems) =>
      currentItems.map((item) =>
        item.product.id === productId
          ? {
              ...item,
              quantity: item.quantity + 1,
            }
          : item
      )
    );
  }

  function decreaseQuantity(productId: number) {
    setCartItems((currentItems) =>
      currentItems.map((item) =>
        item.product.id === productId
          ? {
              ...item,
              quantity: Math.max( // fonks ile urun adedi 1'in altina inmez
                1,
                item.quantity - 1
              ),
            }
          : item
      )
    );
  }

  /* Belirtilen ürünü sepetten tamamen kaldırır. */
  function removeFromCart(productId: number) {
    setCartItems((currentItems) =>
      currentItems.filter(
        (item) => item.product.id !== productId
      )
    );
  }

  /* Sepetteki bütün ürünleri kaldırır. */
  function clearCart() {
    setCartItems([]);
  }

  // toplam urun adedi
  const totalQuantity = cartItems.reduce(
    (total, item) => total + item.quantity,
    0
  );

  // toplam fiyat
  const totalPrice = cartItems.reduce(
    (total, item) =>
      total + item.product.price * item.quantity,
    0
  );

  return (
    <CartContext.Provider
      value={{
        cartItems,
        addToCart,
        increaseQuantity,
        decreaseQuantity,
        removeFromCart,
        clearCart,
        totalQuantity,
        totalPrice,
      }}
    >
      {children}
    </CartContext.Provider>
  );
}

/*
  useCart sayesinde bileşenler sepet bilgilerine
  ve sepet fonksiyonlarına kolayca ulaşabilir.
*/
export function useCart() {
  const context = useContext(CartContext);

  if (!context) {
    throw new Error(
      "useCart, CartProvider içerisinde kullanılmalıdır."
    );
  }

  return context;
}
    