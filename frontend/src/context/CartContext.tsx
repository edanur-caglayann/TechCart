/* Bu dosyada useState, useEffect ve localStorage kullanilir. 
Bunlar tarayici tarafinda calistigi icin dosyayi Client compoent
oalrak isaretleriz
*/

"use client";

import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";

import type { CartItem } from "../types/cart";
import type { Product } from "../types/product";

// Yapilabilecek sepet islemlerinin tiplerini belirleriz
type CartContextType = {
  cartItems: CartItem[];

  addToCart: (
    product: Product,
    quantity?: number
  ) => void;

  increaseQuantity: (productId: number) => void; // miktari arttir

  decreaseQuantity: (productId: number) => void; // miktari azlt

  removeFromCart: (productId: number) => void;

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
}: CartProviderProps) {
  /* cartItems -> sepetin mevcut durumu
  setCartItems -> sepeti degistirmek icin kullandigimiz fonks
  */

  const [cartItems, setCartItems] = useState<CartItem[]>(
    []
  );

  /*
  sayfa ilk acildiginda localStorage icindeki
  onceden kaydedilen sepeti okuyarak cartItems state'ine aktarir
  Syafa yenilendiginde kullanici bilgileri silinmez.
  localStorage ile tarayicidan girilen kullanici bilgileri bilgisayarda kallici olarak
  saklar. Ama kart bilgileri gibi ozel veriler hairc.
  SEPETI KAYDEDERIZ
  */

  // syafa yenilendiginde getItem ile kaydedilmis sepeti okuruz

  useEffect(() => {
    const savedCart = localStorage.getItem(
      "techcart-cart"
    );

    /* localStorage verileri metin olarak sakladigi icin
    metni tekrar js dizisine donustururuz
    */

    if (savedCart) {
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

  // sepet bilgileirni tarayicicya setItem ile kaydedriz

  useEffect(() => {
    localStorage.setItem(
      "techcart-cart",
      JSON.stringify(cartItems)
    );
  }, [cartItems]);

  /*
  Sepete ürün ekleme işlemi.
  quantity parametresi gönderilmezse
  varsayılan olarak bir adet ürün eklenir.
  */

  function addToCart(
    product: Product,
    quantity = 1
  ) {
    setCartItems((currentItems) => {
      /*
        Sepete eklenecek miktarın birden
        küçük olmasını engelleriz.
      */

      const quantityToAdd = Math.max(1, quantity);

      /*
        Ürün için stok miktarı tanımlanmışsa onu,
        tanımlanmamışsa sınırsız değeri kullanırız.
        Gerçek stok kontrolü daha sonra backend
        tarafından da yapılacaktır.
      */

      const maximumQuantity =
        product.stockQuantity ??
        Number.POSITIVE_INFINITY;

      /*
        Ürünün daha önce sepete eklenip
        eklenmediğini id değeriyle kontrol ederiz.
      */

      const existingItem = currentItems.find(
        (item) => item.product.id === product.id
      );

        // Ürün zaten sepetteyse yeni bir satır oluşturmak yerine mevcut ürünün adedini artırırız.
      if (existingItem) {
        return currentItems.map((item) =>
          item.product.id === product.id
            ? {
                ...item,

                /*
                  Mevcut adet ile eklenecek adedi toplarız.
                  Math.min sayesinde sonuç ürünün stok miktarını geçemez.
                */

                quantity: Math.min(
                  item.quantity + quantityToAdd,
                  maximumQuantity
                ),
              }
            : item
        );
      }
       // Ürün sepette yoksa seçilen adetle yeni sepet elemanı oluştururuz.
      return [
        ...currentItems,
        {
          product,

          quantity: Math.min(
            quantityToAdd,
            maximumQuantity
          ),
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

              quantity: Math.min(
                item.quantity + 1,
                item.product.stockQuantity ??
                  Number.POSITIVE_INFINITY
              ),
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

              quantity: Math.max(
                // fonks ile urun adedi 1'in altina inmez
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