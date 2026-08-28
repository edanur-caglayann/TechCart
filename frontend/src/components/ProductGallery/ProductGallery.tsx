/*
  Seçilen görseli useState ile yönettiğimiz için
  bu bileşeni Client Component olarak işaretleriz.
*/
"use client";

import {
  Disc3,
  Headphones,
  Laptop,
  Smartphone,
  Tablet,
  Tv,
} from "lucide-react";

import Image from "next/image";
import { useState } from "react";

import { Product } from "../../types/product";
import styles from "./ProductGallery.module.css";

/* ProductGallery bileşeninin alacağı ürün tipini tanımlar. */
type ProductGalleryProps = {
  product: Product;
};

/*
  Ürünün henüz gerçek görseli bulunmuyorsa
  visualType değerine göre geçici ikon gösterir.
*/
function ProductFallbackVisual({
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

  // Diğer visualType değerlerinde pikap ikonu gösterilir.
  return <Disc3 size={210} strokeWidth={1} />;
}

export default function ProductGallery({
  product,
}: ProductGalleryProps) {
  /*
    product.images bulunmuyorsa boş bir dizi kullanırız.

    Böylece henüz görsel eklenmemiş ürünlerde
    uygulama hata vermeden geçici ikonu gösterebilir.
  */
  const images = product.images ?? [];

  /*
    selectedImage, büyük alanda gösterilecek görseli tutar.

    Dizinin ilk görseli varsa başlangıçta onu seçeriz.
    Görsel yoksa null değeri kullanılır.
  */
  const [selectedImage, setSelectedImage] = useState<
    string | null
  >(images[0] ?? null);

  return (
    <div className={styles.gallery}>
      {/* Seçilen ürün görselinin gösterildiği büyük alan */}
      <div className={styles.mainVisual}>
        {selectedImage ? (
          <Image
            src={selectedImage}
            alt={`${product.name} ürün görseli`}
            fill
            priority
            sizes="(max-width: 950px) 100vw, 55vw"
            className={styles.productImage}
          />
        ) : (
          /*
            Ürüne ait gerçek görsel bulunmuyorsa
            ürün kategorisine uygun ikon gösterilir.
          */
          <ProductFallbackVisual
            type={product.visualType}
          />
        )}

        {/* Stokta bulunmayan ürün için uyarı etiketi */}
        {!product.inStock && (
          <span className={styles.outOfStockBadge}>
            Tükendi
          </span>
        )}
      </div>

      {/*
        Birden fazla ürün görseli varsa küçük
        görsel seçim butonlarını gösteririz.
      */}
      {images.length > 1 && (
        <div
          className={styles.thumbnailList}
          aria-label="Ürün görselleri"
        >
          {images.map((image, index) => (
            <button
              key={image}
              className={`${styles.thumbnailButton} ${
                selectedImage === image
                  ? styles.activeThumbnail
                  : ""
              }`}
              type="button"
              aria-label={`${index + 1}. ürün görselini göster`}
              aria-pressed={selectedImage === image}
              onClick={() => setSelectedImage(image)}
            >
              <Image
                src={image}
                alt=""
                width={78}
                height={78}
                className={styles.thumbnailImage}
              />
            </button>
          ))}
        </div>
      )}
    </div>
  );
}