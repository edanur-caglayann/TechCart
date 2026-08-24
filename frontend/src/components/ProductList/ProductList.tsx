/* useState, onClick, document gibi tarayici tarafinda calisan 
ozellikler kullaniyoruz. Next.js'e bu dosyanin bir Client 
component oldugunu bildiriyoruz */
"use client";

/* sayfalamada onceki ve sonraki sayfa ikonlari icin */
import {
  ChevronLeft,
  ChevronRight,
} from "lucide-react";

// kullanicinin bulundugu sayfa numarasini hafizada tutmak icin useState kullanilir
import { useState } from "react";

import { products } from "../../data/products";

// Her urunu kart olarak gostermek icin ProductCard bileşenini import ederiz
import ProductCard from "../ProductCard/ProductCard";

// sayfalama butonlari icin gerekli olan css siniflarini import ederiz
import styles from "./ProductList.module.css";

// her sayfada kac urun gosterecegimizi belirleriz
const PRODUCTS_PER_PAGE = 8;

export default function ProductList() {
      /*
    Kullanıcının bulunduğu sayfa numarasını tutar.

    currentPage: Mevcut sayfa numarası
    setCurrentPage: Sayfa numarasını değiştiren fonksiyon
    useState(1): Başlangıçta birinci sayfa gösterilir
     */

    const [currentPage, setCurrentPage] = useState(1);
    const totalPages = Math.ceil(products.length / PRODUCTS_PER_PAGE); // Math.ceil, sonuç küsuratlıysa yukarı yuvarlar.
  
    // secilen sayfada hangi urunden baslanacak
    const firstProductIndex = (currentPage - 1) * PRODUCTS_PER_PAGE;

    /* urunler dizisinden, secilen sayfada gosterecegimiz urunleri slice ile aliyoruz
     Birinci sayfada:
    products.slice(0, 4)

    İkinci sayfada:
    products.slice(4, 8)
    */
    const visibleProducts = products.slice(
      firstProductIndex,
      firstProductIndex + PRODUCTS_PER_PAGE
    );

    // kullancii sayfa numarasinda bstiginda calisan fonk
    function changePage(page: number) {
        setCurrentPage(page); // suanki sayfayi secilen sayfayla ddegistirir

        document
      .getElementById("products")
      ?.scrollIntoView({ behavior: "smooth" });
    }  

   return (
    // Ürün başlığı, kartlar ve sayfalama alanının tamamı
    <div className={styles.productListArea}>
      <div className={styles.listHeader}>
        <div>
          <h2>Ürünleri Keşfet</h2>

          <p>{products.length} ürün gösteriliyor</p>
        </div>

        <select
          className={styles.sortSelect}
          aria-label="Ürünleri sırala"
          defaultValue="recommended"
        >
    
          <option value="recommended">
            Önerilen sıralama
          </option>

          <option value="price-asc">
            Fiyat: Artan
          </option>

          <option value="price-desc">
            Fiyat: Azalan
          </option>

          <option value="rating">
            En yüksek puan
          </option>
        </select>
      </div>

      {/* Mevcut sayfadaki dört ürün kartını tutan grid alanı */}
      <div className={styles.productGrid}>
        {/*
          map, visibleProducts dizisindeki her ürünü sırayla dolaşır.
          Her ürün için bir ProductCard bileşeni oluşturur.
        */}
        {visibleProducts.map((product) => (
          <ProductCard
            /*
              React'in oluşturulan kartları birbirinden
              ayırt edebilmesi için benzersiz key veriyoruz.
            */
            key={product.id}

            /*
              Mevcut ürün bilgisini ProductCard
              bileşenine gönderiyoruz.
            */
            product={product}
          />
        ))}
      </div>

      {/* Sayfa numaraları ve ileri-geri butonlarını tutan alan.. */}
      <nav
        className={styles.pagination}
        aria-label="Ürün sayfaları"
      >
        {/* Önceki sayfaya geçme butonu */}
        <button
          type="button"
          aria-label="Önceki sayfa"

          /*
            Kullanıcı birinci sayfadaysa önceki sayfa bulunmadığı için buton devre dışı bırakılır.
          */
          disabled={currentPage === 1}

          /*
            Butona tıklandığında mevcut sayfa
            numarasından bir çıkarılır.
          */
          onClick={() => changePage(currentPage - 1)}
        >
          <ChevronLeft size={19} />
        </button>

        {/*
          totalPages kadar sayfa numarası butonu oluşturuyoruz. */}
        {Array.from(
          { length: totalPages },
          (_, index) => {
        
            const page = index + 1;

            return (
              <button
                key={page}

                type="button"

                /*
                  Butonun sayfa numarası mevcut sayfaya eşitse
                  activePage CSS sınıfı uygulanarak secili sayfa siyah forunur
                */
                className={
                  currentPage === page
                    ? styles.activePage
                    : ""
                }
                // tiklaninca ilgili sayfaya gecilir
                onClick={() => changePage(page)}
              >
                {page}
              </button>
            );
          }
        )}

        <button
          type="button"
          aria-label="Sonraki sayfa"

          /*
            son sayfadayken sonraki sayfa bulunmadığı için buton devre dışı bırakılır.
          */
          disabled={currentPage === totalPages}

          /*
            Butona tıklandığında mevcut sayfa numarasına bir eklenir.
          */
          onClick={() => changePage(currentPage + 1)}
        >
          <ChevronRight size={19} />
        </button>
      </nav>
    </div>
  );
}