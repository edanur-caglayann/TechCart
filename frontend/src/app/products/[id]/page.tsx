import {
  ArrowLeft,
  Check,
  Disc3,
  Headphones,
  Laptop,
  PackageCheck,
  ShieldCheck,
  Smartphone,
  Star,
  Tablet,
  Truck,
  Tv,
} from "lucide-react";

import Link from "next/link";
import { notFound } from "next/navigation";

import { products } from "../../../data/products";
import { Product } from "../../../types/product";
import AddToCartButton from "../../../components/AddToCartButton/AddToCartButton";
import styles from "./page.module.css";


/*
  Ürünün visualType değerine göre
  detay sayfasında gösterilecek ikonu belirler.
*/
function ProductVisual({
  type,
}: {
  type: Product["visualType"];
}) {
  if (type === "phone") {
    return <Smartphone size={190} strokeWidth={1} />;
  }

  if (type === "computer") {
    return <Laptop size={230} strokeWidth={1} />;
  }

  if (type === "tablet") {
    return <Tablet size={200} strokeWidth={1} />;
  }

  if (type === "television") {
    return <Tv size={230} strokeWidth={1} />;
  }

  if (type === "headphone") {
    return <Headphones size={210} strokeWidth={1} />;
  }

  /*
    Diğer visualType değerlerinde pikap ikonu gösterilir.
  */
  return <Disc3 size={210} strokeWidth={1} />;
}

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
        <div className={styles.visualArea}>
          <ProductVisual type={product.visualType} />

          {!product.inStock && (
            <span className={styles.outOfStockBadge}>
              Tükendi
            </span>
          )}
        </div>

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

          <div className={styles.ratingArea}>
            <div className={styles.rating}>
              <Star
                size={19}
                fill="currentColor"
                strokeWidth={1.5}
              />

              <strong>{product.rating}</strong>
            </div>

            <span>
              ({product.reviewCount} değerlendirme)
            </span>
          </div>

          <p className={styles.description}>
            {product.name}, günlük kullanım ve teknoloji
            ihtiyaçları için yüksek performans, güvenilirlik
            ve modern tasarımı bir araya getirir. Ürün
            özelliklerini inceleyerek ihtiyacına uygun olup
            olmadığını değerlendirebilirsin.
          </p>

          <div className={styles.priceArea}>
            <span>Ürün fiyatı</span>

            <strong>
              {product.price.toLocaleString("tr-TR")} ₺
            </strong>
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

      {/*
        Ürünün temel özelliklerini gösteren alt bölüm.
        Şimdilik örnek özellikler kullanıyoruz.
      */}
      <section className={styles.featuresSection}>
        <div className={styles.sectionTitle}>
          <span>ÜRÜN BİLGİLERİ</span>
          <h2>Ürün özellikleri</h2>
        </div>

        <div className={styles.featuresGrid}>
          <div className={styles.featureItem}>
            <Check size={20} />
            <span>
              <strong>Marka</strong>
              {product.brand}
            </span>
          </div>

          <div className={styles.featureItem}>
            <Check size={20} />
            <span>
              <strong>Kategori</strong>
              {product.category}
            </span>
          </div>

          <div className={styles.featureItem}>
            <Check size={20} />
            <span>
              <strong>Model</strong>
              {product.model}
            </span>
          </div>

          <div className={styles.featureItem}>
            <Check size={20} />
            <span>
              <strong>Stok durumu</strong>

              {product.inStock
                ? "Stokta var"
                : "Stokta yok"}
            </span>
          </div>
        </div>
      </section>
    </main>
  );
}