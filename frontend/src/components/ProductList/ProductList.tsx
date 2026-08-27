/*
  Next.js App Router mimarisinde URL parametreleri (useSearchParams) ve
  yönlendirme (useRouter) hook'ları ile tarayıcı nesneleri (document)
  kullanıldığı için bu bileşenin bir Client Component olduğunu belirtiyoruz.
*/

"use client";

import {
  ChevronLeft,
  ChevronRight,
} from "lucide-react";

import {
  type ChangeEvent,
  useEffect,
  useState,
} from "react";

import {
  useRouter,
  useSearchParams,
} from "next/navigation";

import { products } from "../../data/products";

// Her ürünü kart olarak göstermek için ProductCard bileşenini import ederiz.
import ProductCard from "../ProductCard/ProductCard";

// Sayfalama butonları için gerekli olan CSS sınıflarını import ederiz.
import styles from "./ProductList.module.css";

import ProductSkeleton from "../ProductSkeleton/ProductSkeleton";
import ProductEmptyState from "../ProductEmptyState/ProductEmptyState";

// Kullanıcının sayfa başına seçebileceği ürün sayılarını belirleriz.
const PAGE_SIZE_OPTIONS = [10, 20, 30];

// Yüklenme esnasında ekranda 8 geçici ürün kartı gösteririz.
const SKELETON_COUNT = 8;

export default function ProductList() {
  const router = useRouter();
  const searchParams = useSearchParams();

  // Ürünlerin yüklenip yüklenmediği durumunu tutar.
  const [isLoading, setIsLoading] = useState(true);

  /*
    URL'deki "page" parametresini okur.
    Parametre bulunmazsa varsayılan olarak birinci sayfayı kullanır.
  */
  const currentPage =
    Number(searchParams.get("page")) || 1;

  /*
    URL içerisindeki pageSize parametresini okur.

    Örnek:
    /?page=1&pageSize=20

    Parametre bulunmazsa varsayılan olarak
    sayfada 10 ürün gösterilir.
  */
  const requestedPageSize =
    Number(searchParams.get("pageSize")) || 10;

  /*
    URL üzerinden yalnızca izin verilen değerlerden
    biri kabul edilir.

    Geçersiz bir değer gönderilirse 10 kullanılır.
  */
  const pageSize = PAGE_SIZE_OPTIONS.includes(
    requestedPageSize
  )
    ? requestedPageSize
    : 10;

  /*
    Backend bağlantısı olana kadar geçici
    1.5 saniyelik bekleme süresi oluştururuz.

    Sayfa numarası veya sayfada gösterilecek ürün
    sayısı değiştiğinde yüklenme görünümü yeniden çalışır.
  */
  useEffect(() => {
    setIsLoading(true);

    const loadingTimer = window.setTimeout(() => {
      setIsLoading(false);
    }, 1500);

    // Süre tamamlanmadan başka sayfaya geçilirse timer temizlenir.
    return () => {
      window.clearTimeout(loadingTimer);
    };
  }, [currentPage, pageSize]);

  // Seçilen ürün sayısına göre toplam sayfa sayısını hesaplar.
  const totalPages = Math.ceil(
    products.length / pageSize
  );

  // Seçilen sayfada gösterilecek ilk ürünün indeksini hesaplar.
  const firstProductIndex =
    (currentPage - 1) * pageSize;

  // İlgili sayfada gösterilecek ürünleri seçer.
  const visibleProducts = products.slice(
    firstProductIndex,
    firstProductIndex + pageSize
  );

  // Kullanıcı sayfa numarasına bastığında çalışan fonksiyon.
  function changePage(page: number) {
    /*
      Sayfa değiştirilirken kullanıcının seçtiği
      pageSize değeri URL içerisinde korunur.
    */
    router.push(
      `/?page=${page}&pageSize=${pageSize}#products`
    );

    // Ürünler bölümüne yumuşak biçimde kaydırır.
    document
      .getElementById("products")
      ?.scrollIntoView({ behavior: "smooth" });
  }

  /*
    Kullanıcı sayfa başına gösterilecek ürün
    sayısını değiştirdiğinde çalışır.
  */
  function changePageSize(
    event: ChangeEvent<HTMLSelectElement>
  ) {
    // Select içerisinden gelen metin değerini sayıya çevirir.
    const newPageSize = Number(event.target.value);

    /*
      Ürün sayısı değiştiğinde kullanıcı birinci
      sayfaya döndürülür.

      Seçilen değer daha sonra backend'e pageSize
      parametresi olarak gönderilebilir.
    */
    router.push(
      `/?page=1&pageSize=${newPageSize}#products`
    );

    // Ürünler bölümüne yumuşak biçimde kaydırır.
    document
      .getElementById("products")
      ?.scrollIntoView({ behavior: "smooth" });
  }

  return (
    // Ürün başlığı, kartlar ve sayfalama alanının tamamı.
    <div className={styles.productListArea}>
      <div className={styles.listHeader}>
        <div>
          <h2>Ürünleri Keşfet</h2>

          {/*
            Yalnızca mevcut sayfada gösterilen
            ürünlerin sayısını kullanıcıya bildirir.
          */}
          <p>
            {visibleProducts.length} ürün gösteriliyor
          </p>
        </div>

        {/* Sayfa boyutu ve sıralama alanlarını tutar */}
        <div className={styles.listControls}>
          {/*
            Kullanıcının sayfada görmek istediği
            ürün sayısını seçmesini sağlar.
          */}
          <label className={styles.pageSizeControl}>
            <span>Sayfada göster</span>

            <select
              className={styles.pageSizeSelect}
              aria-label="Sayfa başına ürün sayısı"
              value={pageSize}
              onChange={changePageSize}
            >
              {PAGE_SIZE_OPTIONS.map((option) => (
                <option
                  key={option}
                  value={option}
                >
                  {option}
                </option>
              ))}
            </select>
          </label>

          {/* Ürün sıralama seçenekleri */}
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
          </select>
        </div>
      </div>

      {/* Ekran okuyucular için yüklenme ve durum bildirim metni */}
      <p
        className={styles.screenReaderText}
        role="status"
        aria-live="polite"
      >
        {isLoading
          ? "Ürünler yükleniyor."
          : visibleProducts.length === 0
            ? "Arama veya filtreleme sonucunda ürün bulunamadı."
            : "Ürünler yüklendi."}
      </p>

      {/* Yüklenme durumu, boş durum veya ürün listeleme kontrolü */}
      {isLoading ? (
        <div
          className={styles.productGrid}
          aria-busy="true"
        >
          {Array.from(
            { length: SKELETON_COUNT },
            (_, index) => (
              <ProductSkeleton
                key={`product-skeleton-${index}`}
              />
            )
          )}
        </div>
      ) : visibleProducts.length === 0 ? (
        <ProductEmptyState />
      ) : (
        <div
          className={styles.productGrid}
          aria-busy="false"
        >
          {visibleProducts.map((product) => (
            <ProductCard
              key={product.id}
              product={product}
            />
          ))}
        </div>
      )}

      {/* Sayfalama butonları alanı */}
      {!isLoading && visibleProducts.length > 0 && (
        <nav
          className={styles.pagination}
          aria-label="Ürün sayfaları"
        >
          {/* Önceki sayfaya geçme butonu */}
          <button
            type="button"
            aria-label="Önceki sayfa"
            disabled={currentPage === 1}
            onClick={() =>
              changePage(currentPage - 1)
            }
          >
            <ChevronLeft size={19} />
          </button>

          {/* Toplam sayfa sayısı kadar buton oluşturulur */}
          {Array.from(
            { length: totalPages },
            (_, index) => {
              const page = index + 1;

              return (
                <button
                  key={page}
                  type="button"
                  className={
                    currentPage === page
                      ? styles.activePage
                      : ""
                  }
                  onClick={() => changePage(page)}
                >
                  {page}
                </button>
              );
            }
          )}

          {/* Sonraki sayfaya geçme butonu */}
          <button
            type="button"
            aria-label="Sonraki sayfa"
            disabled={currentPage === totalPages}
            onClick={() =>
              changePage(currentPage + 1)
            }
          >
            <ChevronRight size={19} />
          </button>
        </nav>
      )}
    </div>
  );
}