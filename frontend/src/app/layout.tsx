import type { Metadata } from "next";

import {
  Geist,
  Geist_Mono,
} from "next/font/google";

import { AuthProvider } from "../context/AuthContext";
import { CartProvider } from "../context/CartContext";
import { AddressProvider } from "../context/AddressContext";
import { OrderProvider } from "../context/OrderContext";

import "./globals.css";


//Projede kullanılacak ana yazı tipi
const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

/* Kod veya teknik metinler için monospace yazı tipi. */
const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

//Tarayıcı sekmesinde gösterilecek başlık ve açıklama bilgileri.
export const metadata: Metadata = {
  title: "TechCart",
  description:
    "Teknoloji ürünlerini keşfetmenin kolay yolu.",
};

//RootLayout, projedeki bütün sayfaları kapsayan en üst yerleşim bileşenidir.
export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="tr">
      <body
        className={`
          ${geistSans.variable}
          ${geistMono.variable}
        `}
      >
        <AuthProvider> {/* bütün uygulamayı sardığı için giriş, kayıt ve Header kullanıcı bilgisine ulaşabilir.*/}
          <AddressProvider>{/* kayıtlı adresleri ortak olarak bütün sayfalara ulaştırır. */}
            <CartProvider> {/* bütün sayfalarda ortak sepet bilgisinin kullanılmasını sağlar.*/}
              <OrderProvider> {/* bütün sayfalarda ortak sipariş bilgisinin kullanılmasını sağlar.*/}
                {children}
              </OrderProvider>
            </CartProvider>
          </AddressProvider>
        </AuthProvider>
      </body>
    </html>
  );
}