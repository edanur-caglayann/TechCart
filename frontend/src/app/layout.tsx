import type { Metadata } from "next";
import {
  Geist,
  Geist_Mono,
} from "next/font/google";

import { CartProvider } from "../context/CartContext";
import "./globals.css";

/*
  Projede kullanılacak ana yazı tipini tanımlar.
  variable değeri globals.css içerisinde kullanılabilir.
*/
const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

/* Kod veya teknik metinler için monospace yazı tipi */
const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

/*
  Tarayıcı sekmesinde gösterilecek
  başlık ve açıklama bilgileri.
*/
export const metadata: Metadata = {
  title: "TechCart",
  description:
    "Teknoloji ürünlerini keşfetmenin kolay yolu.",
};

/*
  RootLayout, projedeki bütün sayfaları kapsayan
  en üst yerleşim bileşenidir.

  Ana sayfa, ürün detayı, sepet, giriş ve kayıt
  sayfalarının tamamı bu layout içerisinden geçer.
*/
export default function RootLayout({
  children,
}: Readonly<{
  /*
    children, kullanıcının o anda görüntülediği
    sayfayı temsil eder.
  */
  children: React.ReactNode;
}>) {
  return (
    /*
      lang="tr", sayfanın dilinin Türkçe
      olduğunu tarayıcıya bildirir.
    */
    <html lang="tr">
      <body
        className={`
          ${geistSans.variable}
          ${geistMono.variable}
        `}
      >
        {/*
          CartProvider bütün sayfaları sardığı için
          ProductCard, Header, ürün detay sayfası ve
          CartPage aynı sepet bilgilerine ulaşabilir.
        */}
        <CartProvider>
          {children}
        </CartProvider>
      </body>
    </html>
  );
}