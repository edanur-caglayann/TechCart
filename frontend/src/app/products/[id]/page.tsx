import {
  ArrowLeft,
  Check,
  PackageCheck,
  ShieldCheck,
  Truck,
} from "lucide-react";

import Link from "next/link";

import { notFound } from "next/navigation";

import { products } from "../../../data/products";

import ProductGallery from "../../../components/ProductGallery/ProductGallery";

import AddToCartButton from "../../../components/AddToCartButton/AddToCartButton";

import {
  calculateIncludedVat,
  calculateNetAmount,
  formatCurrency,
} from "../../../utils/tax";

import styles from "./page.module.css";

/*
  [id] dinamik klasöründen gelen adres bilgisinin tipidir.
  adres /products/3 ise: params içindeki id değeri "3" olur.
*/
type ProductDetailPageProps = {
  params: Promise<{
    id: string;
  }>;
};

export default async function ProductDetailPage({
  params,
}: ProductDetailPageProps) {
  /*
    Next.js tarafından gönderilen dinamik adres parametresini alıyoruz.
  */
  const { id } = await params;

  /*
    URL'den gelen id metin tipinde. Number kullanarak sayıya dönüştürüyoruz.
  */
  const productId = Number(id);

  /*
    products dizisi içerisinde id değeri adresle eşleşen ürünü arıyoruz.
  */
  const product = products.find(
    (item) => item.id === productId
  );

  /*
    ürün bulunamazsa 404
  */
  if (!product) {
    notFound();
  }

  /* ?? operatoru ile -> urun varsa urun ozelliklerini kullan
  yoksa varsayilan 4 ozellgi kullan 
  */
  const technicalSpecifications =
    product.technicalSpecifications ?? [
      {
        label: "Marka",
        value: product.brand,
      },
      {
        label: "Kategori",
        value: product.category,
      },
      {
        label: "Model",
        value: product.model,
      },
      {
        label: "Stok durumu",
        value: product.inStock
          ? "Stokta var"
          : "Stokta yok",
      },
    ];

  // Ürünün KDV dahil fiyatının içerisindeki KDV tutarini ürünün kendi oranına göre hesaplar
  const includedVat = calculateIncludedVat(
    product.price,
    product.vatRate
  );


  // Ürünün KDV dahil satış fiyatından KDV hariç fiyatını hesaplar.
  const netPrice = calculateNetAmount(
    product.price,
    product.vatRate
  );

  return (
    <main className={styles.detailPage}>
      {/*
        Sayfanın üst kısmında logo ve ana sayfaya dönüş bağlantısı bulunur.
      */}
      <header className={styles.topBar}>
        <Link className={styles.logo} href="/">
          TECHCART
        </Link>

        <Link
          className={styles.backLink}
          href="/#products"
        >
          <ArrowLeft size={18} />
          Ürünlere dön
        </Link>
      </header>

      {/*
        Ürün görseliyle ürün bilgilerini
        yan yana gösteren ana alan.
      */}
      <section className={styles.productDetail}>
        {/*
          Ürünün çoklu görsellerini veya görsel
          bulunmuyorsa geçici ürün ikonunu gösterir.
        */}
        <ProductGallery product={product} />

        <div className={styles.informationArea}>
          <div className={styles.productMeta}>
            <span>{product.brand}</span>

            <span>•</span>

            <span>{product.category}</span>
          </div>

          <h1>{product.name}</h1>

          <p className={styles.model}>
            Model: {product.model}
          </p>

          <p className={styles.description}>
            {/* ?? ile -> urune ozel aciklama varsa onu goster
            yoksa varsayilan aciklamayi goster */}
            {product.description ??
              `${product.name}, günlük kullanım ve teknoloji ihtiyaçları için performans, güvenilirlik ve modern tasarımı bir araya getirir.`}
          </p>

          <div className={styles.priceArea}>
            <span>KDV dahil satış fiyatı</span>

            <strong>
              {formatCurrency(product.price)}
            </strong>

            <div className={styles.taxInformation}>
              <span>
                KDV hariç fiyat:{" "}
                <strong>
                  {formatCurrency(netPrice)}
                </strong>
              </span>

              <span>
                KDV oranı: %{product.vatRate}
              </span>

              <span>
                Fiyata dahil KDV:{" "}
                <strong>
                  {formatCurrency(includedVat)}
                </strong>
              </span>
            </div>
          </div>

          <div className={styles.stockInformation}>
            <PackageCheck size={20} />

            <span>
              {product.inStock
                ? "Ürün stokta ve gönderime hazır."
                : "Bu ürün şu anda stokta bulunmuyor."}
            </span>
          </div>

          {/* boylece hem urun kartindaki 
          hem de urun detay sayfasindaki buton ayni sepete urun ekler */}
          <AddToCartButton
            product={product}
            className={styles.cartButton}
            showCartLink
          />

          <div className={styles.serviceInformation}>
            <div>
              <Truck size={22} />

              <span>
                <strong>Hızlı teslimat</strong>
                Güvenli kargo seçeneği
              </span>
            </div>

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
          {/* technicalSpecifications dizisindeki her teknik özellik için bir özellik kutusu oluştururuz. */}
          {technicalSpecifications.map(
            (specification) => (
              <div
                className={styles.featureItem}
                key={specification.label}
              >
                <Check size={20} />

                <span>
                  <strong>
                    {specification.label}
                  </strong>

                  {specification.value}
                </span>
              </div>
            )
          )}
        </div>
      </section>
    </main>
  );
}