
"use client";

import {
  ArrowLeft,
  Check,
  PackageCheck,
  ShieldCheck,
  Truck,
} from "lucide-react";

import Link from "next/link";
import { notFound } from "next/navigation";
import { useParams } from "next/navigation";
import { useEffect, useState } from "react";

import { getProductDetailRequest } from "../../../services/productService";
import type { ProductDetail } from "../../../types/productDetail";

import ProductGallery from "../../../components/ProductGallery/ProductGallery";
import AddToCartButton from "../../../components/AddToCartButton/AddToCartButton";
import { formatCurrency } from "../../../utils/tax";

import styles from "./page.module.css";

export default function ProductDetailPage() {

  const { id } = useParams<{ id: string }>();

  const [product, setProduct] = useState<ProductDetail | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [notFoundError, setNotFoundError] = useState(false);

  useEffect(() => {
    let isCancelled = false;

    async function loadProduct() {
      setIsLoading(true);
      try {
        const result = await getProductDetailRequest(id);
        if (!isCancelled) setProduct(result);
      } catch {
        // Backend 404 dönerse (ürün yok) buraya düşer.
        if (!isCancelled) setNotFoundError(true);
      } finally {
        if (!isCancelled) setIsLoading(false);
      }
    }

    loadProduct();
    return () => { isCancelled = true; };
  }, [id]);

  if (notFoundError) {
    notFound();
  }

  if (isLoading || !product) {
    return (
      <main className={styles.detailPage}>
        <p>Ürün yükleniyor...</p>
      </main>
    );
  }


  const technicalSpecifications = [
    { label: "Marka", value: product.brand },
    { label: "Kategori", value: product.category },
    { label: "Model", value: product.model || "Belirtilmemiş" },
    { label: "Stok durumu", value: product.inStock ? "Stokta var" : "Stokta yok" },
  ];

  return (
    <main className={styles.detailPage}>
      <header className={styles.topBar}>
        <Link className={styles.logo} href="/">
          TECHCART
        </Link>

        <Link className={styles.backLink} href="/#products">
          <ArrowLeft size={18} />
          Ürünlere dön
        </Link>
      </header>

      <section className={styles.productDetail}>
        <ProductGallery product={product} />

        <div className={styles.informationArea}>
          <div className={styles.productMeta}>
            <span>{product.brand}</span>
            <span>•</span>
            <span>{product.category}</span>
          </div>

          <h1>{product.name}</h1>

          <p className={styles.model}>Model: {product.model || "Belirtilmemiş"}</p>

          <p className={styles.description}>

            {product.description || `${product.name}, günlük kullanım ve teknoloji ihtiyaçları için performans, güvenilirlik ve modern tasarımı bir araya getirir.`}
          </p>

          <div className={styles.priceArea}>
            <span>KDV dahil satış fiyatı</span>

            <strong>{formatCurrency(product.price)}</strong>

            <div className={styles.taxInformation}>
              <span>
                KDV hariç fiyat: <strong>{formatCurrency(product.priceWithoutVat)}</strong>
              </span>

              <span>KDV oranı: %{Math.round(product.vatRate * 100)}</span>

              <span>
                Fiyata dahil KDV: <strong>{formatCurrency(product.vatAmount)}</strong>
              </span>
            </div>
          </div>

          <div className={styles.stockInformation}>
            <PackageCheck size={20} />
            <span>
              {product.inStock ? "Ürün stokta ve gönderime hazır." : "Bu ürün şu anda stokta bulunmuyor."}
            </span>
          </div>

          <AddToCartButton
            product={product}
            className={styles.cartButton}
            showCartLink
          />

          <div className={styles.serviceInformation}>
            {/* isReadyToShip ve hasFastDelivery artık İKİ AYRI kutu —
                her biri kendi koşuluna göre bağımsız gösteriliyor/gizleniyor. */}
            {product.isReadyToShip && (
              <div>
                <PackageCheck size={22} />
                <span>
                  <strong>Gönderime hazır</strong>
                  Hemen kargoya verilir
                </span>
              </div>
            )}

            {product.hasFastDelivery && (
              <div>
                <Truck size={22} />
                <span>
                  <strong>Hızlı teslimat</strong>
                  Güvenli kargo seçeneği
                </span>
              </div>
            )}

            <div>
              <ShieldCheck size={22} />
              <span>
                <strong>Güvenli alışveriş</strong>
                Korumalı ödeme altyapısı
              </span>
            </div>
          </div>
        </div>
      </section>

      <section className={styles.featuresSection}>
        <div className={styles.sectionTitle}>
          <span>ÜRÜN BİLGİLERİ</span>
          <h2>Ürün özellikleri</h2>
        </div>

        <div className={styles.featuresGrid}>
          {technicalSpecifications.map((specification) => (
            <div className={styles.featureItem} key={specification.label}>
              <Check size={20} />
              <span>
                <strong>{specification.label}</strong>
                {specification.value}
              </span>
            </div>
          ))}
        </div>
      </section>
    </main>
  );
}