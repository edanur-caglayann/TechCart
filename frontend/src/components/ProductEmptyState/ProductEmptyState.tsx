import { RotateCcw, SearchX } from "lucide-react";
import Link from "next/link";

import styles from "./ProductEmptyState.module.css";

/*
  Arama veya filtreleme sonucunda gösterilecek
  ürün bulunamadığında bu bileşen ekrana gelir.
*/
export default function ProductEmptyState() {
  return (
    /*
      role="status", ürün bulunamadığı bilgisinin
      ekran okuyucular tarafından algılanmasını sağlar.
    */
    <section
      className={styles.emptyState}
      role="status"
      aria-labelledby="empty-state-title"
    >
      {/* Ürün bulunamadığını temsil eden ikon */}
      <div
        className={styles.iconArea}
        aria-hidden="true"
      >
        <SearchX
          size={42}
          strokeWidth={1.5}
        />
      </div>

      {/* Boş durum başlığı */}
      <h3
        id="empty-state-title"
        className={styles.title}
      >
        Ürün bulunamadı
      </h3>

      {/* Kullanıcıya ne yapabileceğini açıklayan metin */}
      <p className={styles.description}>
        Arama ifadenizi veya seçtiğiniz filtreleri
        değiştirerek yeniden deneyebilirsiniz.
      </p>

      {/*
        Kullanıcıyı filtre ve arama parametreleri
        bulunmayan ana ürün listesine yönlendirir.
      */}
      <Link
        href="/#products"
        className={styles.clearButton}
      >
        <RotateCcw
          size={18}
          strokeWidth={1.7}
          aria-hidden="true"
        />

        Tüm ürünleri görüntüle
      </Link>
    </section>
  );
}