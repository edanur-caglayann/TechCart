"use client";

import {
  Camera,
  Cpu,
  Disc3,
  Headphones,
  Keyboard,
  Laptop,
  Mouse,
  Smartphone,
  Speaker,
  Tablet,
  Tv,
  Watch,
} from "lucide-react";

import Image from "next/image";
import { useState } from "react";

import type { ProductDetail } from "../../types/productDetail"; // Product yerine ProductDetail
import styles from "./ProductGallery.module.css";

type ProductGalleryProps = {
  product: ProductDetail;
};


function ProductFallbackVisual({ category }: { category: string }) {
  switch (category) {
    case "Bilgisayar": return <Laptop size={230} strokeWidth={1} />;
    case "Tablet": return <Tablet size={200} strokeWidth={1} />;
    case "Klavye": return <Keyboard size={210} strokeWidth={1} />;
    case "Mouse": return <Mouse size={200} strokeWidth={1} />;
    case "Pikap": return <Disc3 size={210} strokeWidth={1} />;
    case "Kulaklık": return <Headphones size={210} strokeWidth={1} />;
    case "Hoparlör": return <Speaker size={210} strokeWidth={1} />;
    case "Televizyon": return <Tv size={230} strokeWidth={1} />;
    case "Akıllı Saat": return <Watch size={200} strokeWidth={1} />;
    case "Kamera": return <Camera size={205} strokeWidth={1} />;
    case "Telefon": return <Smartphone size={190} strokeWidth={1} />;
    default: return <Cpu size={210} strokeWidth={1} />;
  }
}

export default function ProductGallery({
  product,
}: ProductGalleryProps) {
  const images = product.images;

  const [selectedImage, setSelectedImage] = useState<string | null>(
    images[0] ?? null
  );

  return (
    <div className={styles.gallery}>
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
          <ProductFallbackVisual category={product.category} />
        )}

        {!product.inStock && (
          <span className={styles.outOfStockBadge}>
            Tükendi
          </span>
        )}
      </div>

      {images.length > 1 && (
        <div
          className={styles.thumbnailList}
          aria-label="Ürün görselleri"
        >
          {images.map((image, index) => (
            <button
              key={image}
              className={`${styles.thumbnailButton} ${selectedImage === image
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