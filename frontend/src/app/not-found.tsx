import {
  ArrowLeft,
  FileQuestion,
  Home,
} from "lucide-react";
import Link from "next/link";

import styles from "./not-found.module.css";

export default function NotFoundPage() {
  return (
    <main className={styles.notFoundPage}>
      <section className={styles.notFoundCard}>
        <div className={styles.iconArea}>
          <FileQuestion size={46} />
        </div>

        <span className={styles.errorCode}>
          404
        </span>

        <h1>Sayfa bulunamadı</h1>

        <div className={styles.actions}>
          <Link
            className={styles.primaryButton}
            href="/"
          >
            <Home size={18} />
            Ana Sayfaya Dön
          </Link>

          <Link
            className={styles.secondaryButton}
            href="/#products"
          >
            <ArrowLeft size={18} />
            Ürünleri İncele
          </Link>
        </div>
      </section>
    </main>
  );
}