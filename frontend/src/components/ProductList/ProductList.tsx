"use client";

import { ChevronLeft, ChevronRight } from "lucide-react";
import { type ChangeEvent, useEffect, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";

import { listProductsRequest } from "../../services/productService";
import type { ProductListItem } from "../../types/productListItem";

import ProductCard from "../ProductCard/ProductCard";
import styles from "./ProductList.module.css";
import ProductSkeleton from "../ProductSkeleton/ProductSkeleton";
import ProductEmptyState from "../ProductEmptyState/ProductEmptyState";

const PAGE_SIZE_OPTIONS = [10, 20, 30];
const SKELETON_COUNT = 8;

// 8570 ürün 10'luk sayfalarla ~857 sayfa demek — hepsini buton yapmak yerine
// mevcut sayfanın etrafında birkaç numara + baş/son + "..." gösteren, bilinen
// bir sayfalama deseni.
function getPageNumbers(current: number, total: number): (number | "...")[] {
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);

  const pages: (number | "...")[] = [1];
  if (current > 3) pages.push("...");

  const start = Math.max(2, current - 1);
  const end = Math.min(total - 1, current + 1);
  for (let i = start; i <= end; i++) pages.push(i);

  if (current < total - 2) pages.push("...");
  pages.push(total);
  return pages;
}

export default function ProductList() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const [items, setItems] = useState<ProductListItem[]>([]);
  const [totalItems, setTotalItems] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [isLoading, setIsLoading] = useState(true);

  const currentPage = Number(searchParams.get("page")) || 1;
  const requestedPageSize = Number(searchParams.get("pageSize")) || 10;
  const pageSize = PAGE_SIZE_OPTIONS.includes(requestedPageSize) ? requestedPageSize : 10;
  const sortBy = searchParams.get("sortBy") || "relevance";

  // ESKİDEN: sahte setTimeout(1500) ile mock diziyi dilimliyordu. ARTIK:
  // URL'deki tüm filtre/sayfalama/sıralama bilgisiyle gerçek istek atılıyor.
  useEffect(() => {
    let isCancelled = false;

    async function loadProducts() {
      setIsLoading(true);
      try {
        const result = await listProductsRequest({
          q: searchParams.get("q") ?? undefined,
          category: searchParams.get("category") ?? undefined,
          brand: searchParams.get("brand") ?? undefined,
          minPrice: searchParams.get("minPrice") ? Number(searchParams.get("minPrice")) : undefined,
          maxPrice: searchParams.get("maxPrice") ? Number(searchParams.get("maxPrice")) : undefined,
          color: searchParams.get("color") ?? undefined,
          inStock: searchParams.has("inStock") ? true : undefined,
          sortBy, page: currentPage, pageSize,
        });

        if (!isCancelled) {
          setItems(result.items);
          setTotalItems(result.pagination.totalItems);
          setTotalPages(result.pagination.totalPages);
        }
      } finally {
        if (!isCancelled) setIsLoading(false);
      }
    }

    loadProducts();
    return () => { isCancelled = true; };
    // searchParams.toString(): her render'da yeni obje referansı gelmesin diye
    // string'e çevirip bağımlılık olarak veriyoruz, yoksa sonsuz döngü riski olur.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchParams.toString()]);

  function pushParam(updates: Record<string, string>) {
    const params = new URLSearchParams(searchParams.toString());
    Object.entries(updates).forEach(([key, value]) => params.set(key, value));
    router.push(`/?${params.toString()}#products`);
    document.getElementById("products")?.scrollIntoView({ behavior: "smooth" });
  }

  function changePage(page: number) { pushParam({ page: String(page) }); }
  function changePageSize(event: ChangeEvent<HTMLSelectElement>) { pushParam({ pageSize: event.target.value, page: "1" }); }
  // ESKİDEN: bu select'in hiç onChange'i yoktu, işlevsizdi. ARTIK: sortBy URL'e yazılıyor.
  function changeSortBy(event: ChangeEvent<HTMLSelectElement>) { pushParam({ sortBy: event.target.value, page: "1" }); }

  const pageNumbers = getPageNumbers(currentPage, totalPages);

  return (
    <div className={styles.productListArea}>
      <div className={styles.listHeader}>
        <div>
          <h2>Ürünleri Keşfet</h2>
          <p>{totalItems} ürün bulundu</p>
        </div>

        <div className={styles.listControls}>
          <label className={styles.pageSizeControl}>
            <span>Sayfada göster</span>
            <select className={styles.pageSizeSelect} aria-label="Sayfa başına ürün sayısı" value={pageSize} onChange={changePageSize}>
              {PAGE_SIZE_OPTIONS.map((option) => <option key={option} value={option}>{option}</option>)}
            </select>
          </label>

          <select className={styles.sortSelect} aria-label="Ürünleri sırala" value={sortBy} onChange={changeSortBy}>
            <option value="relevance">Önerilen sıralama</option>
            <option value="price_asc">Fiyat: Artan</option>
            <option value="price_desc">Fiyat: Azalan</option>
            <option value="newest">En yeni</option>
          </select>
        </div>
      </div>

      <p className={styles.screenReaderText} role="status" aria-live="polite">
        {isLoading ? "Ürünler yükleniyor." : items.length === 0 ? "Arama veya filtreleme sonucunda ürün bulunamadı." : "Ürünler yüklendi."}
      </p>

      {isLoading ? (
        <div className={styles.productGrid} aria-busy="true">
          {Array.from({ length: SKELETON_COUNT }, (_, index) => <ProductSkeleton key={`product-skeleton-${index}`} />)}
        </div>
      ) : items.length === 0 ? (
        <ProductEmptyState />
      ) : (
        <div className={styles.productGrid} aria-busy="false">
          {items.map((product) => <ProductCard key={product.id} product={product} />)}
        </div>
      )}

      {!isLoading && items.length > 0 && totalPages > 1 && (
        <nav className={styles.pagination} aria-label="Ürün sayfaları">
          <button type="button" aria-label="Önceki sayfa" disabled={currentPage === 1} onClick={() => changePage(currentPage - 1)}>
            <ChevronLeft size={19} />
          </button>

          {pageNumbers.map((page, index) =>
            page === "..." ? (
              <span key={`ellipsis-${index}`} className={styles.ellipsis}>…</span>
            ) : (
              <button key={page} type="button" className={currentPage === page ? styles.activePage : ""} onClick={() => changePage(page)}>{page}</button>
            )
          )}

          <button type="button" aria-label="Sonraki sayfa" disabled={currentPage === totalPages} onClick={() => changePage(currentPage + 1)}>
            <ChevronRight size={19} />
          </button>
        </nav>
      )}
    </div>
  );
}