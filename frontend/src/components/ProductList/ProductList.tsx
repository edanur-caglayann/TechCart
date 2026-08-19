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

// urunlerin sahip olmasi gereken alanlari tanimlayan Product tipini import ederiz
import { Product } from "../../types/product";

// Her urunu kart olarak gostermek icin ProductCard bileşenini import ederiz
import ProductCard from "../ProductCard/ProductCard";

// sayfalama butonlari icin gerekli olan css siniflarini import ederiz
import styles from "./ProductList.module.css";

const products: Product[] = [
   {
    id: 1,
    name: "iPhone 15 Pro",
    model: "A3101",
    brand: "Apple",
    category: "Telefon",
    rating: 4.8,
    reviewCount: 342,
    price: 89999,
    inStock: true,
    visualType: "phone",
  },
  {
    id: 2,
    name: "Galaxy S24 Ultra",
    model: "SM-S928B",
    brand: "Samsung",
    category: "Telefon",
    rating: 4.7,
    reviewCount: 218,
    price: 74999,
    inStock: true,
    visualType: "phone",
  },
  {
    id: 3,
    name: "MacBook Pro 14",
    model: "M3 Pro",
    brand: "Apple",
    category: "Bilgisayar",
    rating: 4.9,
    reviewCount: 186,
    price: 129999,
    inStock: true,
    visualType: "computer",
  },
  {
    id: 4,
    name: "ROG Zephyrus G16",
    model: "GU605",
    brand: "Asus",
    category: "Bilgisayar",
    rating: 4.7,
    reviewCount: 201,
    price: 79999,
    inStock: false,
    visualType: "computer",
  },
  {
    id: 5,
    name: "OLED evo Smart TV",
    model: "OLED55C4",
    brand: "LG",
    category: "Televizyon",
    rating: 4.6,
    reviewCount: 97,
    price: 64999,
    inStock: true,
    visualType: "television",
  },
  {
    id: 6,
    name: "iPad Air",
    model: "M2 11 inç",
    brand: "Apple",
    category: "Tablet",
    rating: 4.8,
    reviewCount: 156,
    price: 39999,
    inStock: true,
    visualType: "tablet",
  },
  {
    id: 7,
    name: "Kablosuz Kulaklık",
    model: "WH-1000XM5",
    brand: "Sony",
    category: "Aksesuar",
    rating: 4.7,
    reviewCount: 284,
    price: 14999,
    inStock: true,
    visualType: "headphone",
  },
  {
    id: 8,
    name: "Tam Otomatik Pikap",
    model: "AT-LP60X",
    brand: "Audio-Technica",
    category: "Pikap",
    rating: 4.5,
    reviewCount: 73,
    price: 12999,
    inStock: true,
    visualType: "turntable",
  },
];

// her sayfada kac urun gosterecegimizi belirleriz
const PRODUCTS_PER_PAGE = 4;

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