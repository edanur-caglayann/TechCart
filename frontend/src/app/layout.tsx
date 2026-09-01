import type { Metadata } from "next";

import {
  Geist,
  Geist_Mono,
} from "next/font/google";

import { AuthProvider } from "../context/AuthContext";
import { CartProvider } from "../context/CartContext";

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
        {/*
          AuthProvider bütün uygulamayı sardığı için
          giriş, kayıt ve Header kullanıcı bilgisine ulaşabilir.

          CartProvider da bütün sayfalarda ortak
          sepet bilgisinin kullanılmasını sağlar.
        */}
        <AuthProvider>
          <CartProvider>
            {children}
          </CartProvider>
        </AuthProvider>
      </body>
    </html>
  );
}