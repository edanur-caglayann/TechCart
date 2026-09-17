"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState, type FormEvent } from "react";

import { getProductFacetsRequest, type ProductFacets } from "../../services/productService";

import styles from "./Filters.module.css";

export default function Filters() {
  const MAX_PRICE = 1_000_000;

  const router = useRouter();
  const searchParams = useSearchParams();

  const [priceError, setPriceError] = useState("");

  // Kategori/marka/renk seçenekleri artık sabit değil, backend'den geliyor.
  const [facets, setFacets] = useState<ProductFacets | null>(null);
  const [isFacetsLoading, setIsFacetsLoading] = useState(true);

  const getParam = (key: string) => searchParams.get(key) || "";

  const getPriceParam = (key: string) => {
    const digits = getParam(key).replace(/\D/g, "").slice(0, 7);
    if (!digits) return "";
    return Math.min(Number(digits), MAX_PRICE).toString();
  };

  // Sadece facet sonucunu ETKİLEYEN parametreleri izliyoruz — page/pageSize/sortBy
  // değişince facet'leri boşuna yeniden sormuyoruz.
  const activeFilterKey = [
    searchParams.get("q"), searchParams.get("category"), searchParams.get("brand"),
    searchParams.get("minPrice"), searchParams.get("maxPrice"), searchParams.get("color"),
    searchParams.get("inStock"),
  ].join("|");

  useEffect(() => {
    let isCancelled = false;

    async function loadFacets() {
      setIsFacetsLoading(true);
      try {
        const result = await getProductFacetsRequest({
          q: searchParams.get("q") ?? undefined,
          category: searchParams.get("category") ?? undefined,
          brand: searchParams.get("brand") ?? undefined,
          minPrice: searchParams.get("minPrice") ? Number(searchParams.get("minPrice")) : undefined,
          maxPrice: searchParams.get("maxPrice") ? Number(searchParams.get("maxPrice")) : undefined,
          color: searchParams.get("color") ?? undefined,
          inStock: searchParams.has("inStock") ? true : undefined,
        });
        if (!isCancelled) setFacets(result);
      } finally {
        if (!isCancelled) setIsFacetsLoading(false);
      }
    }

    loadFacets();
    return () => { isCancelled = true; };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [activeFilterKey]);

  const handlePriceInput = (event: FormEvent<HTMLInputElement>) => {
    const digits = event.currentTarget.value.replace(/\D/g, "").slice(0, 7);
    if (!digits) { event.currentTarget.value = ""; setPriceError(""); return; }
    event.currentTarget.value = Math.min(Number(digits), MAX_PRICE).toString();
    setPriceError("");
  };

  const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const minPriceValue = formData.get("minPrice")?.toString();
    const maxPriceValue = formData.get("maxPrice")?.toString();
    const minPrice = minPriceValue ? Number(minPriceValue) : 0;
    const maxPrice = maxPriceValue ? Number(maxPriceValue) : null;

    if (maxPrice !== null && minPrice > maxPrice) {
      setPriceError("En az fiyat, en fazla fiyattan büyük olamaz.");
      return;
    }
    setPriceError("");

    const params = new URLSearchParams();
    formData.forEach((value, key) => { if (value) params.append(key, value.toString()); });
    router.push(`?${params.toString()}`, { scroll: false });
    };

  const handleReset = () => router.push(window.location.pathname, { scroll: false });
  return (
    <aside className={styles.filterPanel}>
      <div className={styles.filterHeader}>
        <h2>Filtreler</h2>
        <button className={styles.clearButton} type="button" onClick={handleReset}>Temizle</button>
      </div>

      <form key={searchParams.toString()} className={styles.filterForm} onSubmit={handleSubmit}>        <div className={styles.filterGroup}>
        <label className={styles.groupTitle} htmlFor="category">Kategori</label>
        {/*
            backend tek bir category id kabßul ettiği için (Marka'daki gibi) tek
            seçime düşürüldü, seçenekler facet'lerden dinamik geliyor.
          */}
        <select className={styles.select} id="category" name="category" defaultValue={getParam("category")}>
          <option value="">Tüm kategoriler</option>
          {facets?.categories.map((category) => (
            <option key={category.id} value={category.id}>{category.name} ({category.count})</option>
          ))}
        </select>
      </div>

        <div className={styles.filterGroup}>
          <label className={styles.groupTitle} htmlFor="brand">Marka</label>
          <select className={styles.select} id="brand" name="brand" defaultValue={getParam("brand")}>
            <option value="">Tüm markalar</option>
            {facets?.brands.map((brand) => (
              <option key={brand.id} value={brand.id}>{brand.name} ({brand.count})</option>
            ))}
          </select>
        </div>

        <div className={styles.filterGroup}>
          <h3>Fiyat Aralığı</h3>
          <div className={styles.priceInputs}>
            <div>
              <label htmlFor="minPrice">En az</label>
              <input id="minPrice" name="minPrice" type="text" inputMode="numeric" pattern="[0-9]*" maxLength={7}
                placeholder="0 ₺" defaultValue={getPriceParam("minPrice")} onInput={handlePriceInput} aria-invalid={Boolean(priceError)} />
            </div>
            <div>
              <label htmlFor="maxPrice">En fazla</label>
              <input id="maxPrice" name="maxPrice" type="text" inputMode="numeric" pattern="[0-9]*" maxLength={7}
                placeholder="1.000.000 ₺" defaultValue={getPriceParam("maxPrice")} onInput={handlePriceInput} aria-invalid={Boolean(priceError)} />
            </div>
          </div>
          {priceError && <p className={styles.priceError} role="alert">{priceError}</p>}
        </div>

        <div className={styles.filterGroup}>
          <label className={styles.groupTitle} htmlFor="color">Renk</label>
          {/*
            Kategori ile aynı sebepten tek seçime düşürüldü. Renk daireleri
            (siyah/beyaz/gri/mavi için 4 sabit CSS sınıfı) kaldırıldı çünkü
            backend'den gelen renkler çok daha çeşitli (Rose Gold, Space
            Grey gibi) — sadece isim + sayı gösteriyoruz.
          */}
          <select className={styles.select} id="color" name="color" defaultValue={getParam("color")}>
            <option value="">Tüm renkler</option>
            {facets?.colors.map((colorOption) => (
              <option key={colorOption.color} value={colorOption.color}>{colorOption.color}</option>
            ))}
          </select>
        </div>

        <div className={styles.filterGroup}>
          <h3>Stok Durumu</h3>
          <label className={styles.checkboxLabel}>
            <input type="checkbox" name="inStock" value="true" defaultChecked={searchParams.has("inStock")} />
            <span>Yalnızca stokta olanlar</span>
          </label>
        </div>

        <button className={styles.applyButton} type="submit" disabled={isFacetsLoading}>Filtreleri Uygula</button>
      </form>
    </aside>
  );
}