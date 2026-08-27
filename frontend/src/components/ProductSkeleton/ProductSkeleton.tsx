// ProductSkeleton gercek ProductCart bileseni yuklenirken gosterilen gecici urun kartlari

import styles from "./ProductSkeleton.module.css";

/*
  Bu bileşenin içerisinde gerçek ürün adı, fiyatı veya görseli yoktur.
  Bunun yerine kartın yaklaşık yerleşimini gösteren gri alanlar bulunur.
*/
export default function ProductSkeleton() {
  return (
    
    <article
      className={styles.card}
      aria-hidden="true" // gorsel olarak "yukleniyor" gostergesi olmasi icin
    >
      {/* Gerçek ürün kartındaki ürün görselinin geçici karşılığı 
      Ayni anda 2 css sinifi veririz. 
      styles.skeleton: Hareketli gri yüklenme efektini verir.
      styles.visual: Alanın ürün görseli büyüklüğünde olmasını sağlar.
      */}
      <div
        className={`${styles.skeleton} ${styles.visual}`}
      />

      <div className={styles.content}>
        {/*
          Marka ve kategori bilgisinin geleceği alanı
          iki kısa gri çizgiyle temsil eder.
        */}
        <div className={styles.meta}>
          <div
            className={`${styles.skeleton} ${styles.brand}`}
          />

          <div
            className={`${styles.skeleton} ${styles.category}`}
          />
        </div>

        {/* Ürün adının geleceği geçici alan */}
        <div
          className={`${styles.skeleton} ${styles.productName}`}
        />

        {/* Ürün modeli için kullanılan geçici alan */}
        <div
          className={`${styles.skeleton} ${styles.model}`}
        />

        {/* Ürün fiyatının geleceği alan */}
        <div
          className={`${styles.skeleton} ${styles.price}`}
        />

        {/*
          Detayları Gör ve Sepete Ekle butonlarının
          geçici karşılıkları.
        */}
        <div className={styles.actions}>
          <div
            className={`${styles.skeleton} ${styles.button}`}
          />

          <div
            className={`${styles.skeleton} ${styles.button}`}
          />
        </div>
      </div>
    </article>
  );
}