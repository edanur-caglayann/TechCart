import { LoaderCircle } from "lucide-react";

import styles from "./loading.module.css";

export default function LoadingPage() {
  return (
    <main className={styles.loadingPage}>
      <span className={styles.logo}>
        TECHCART
      </span>

      <section className={styles.loadingContent}>
        <div className={styles.iconArea}>
          <LoaderCircle size={38} />
        </div>

        <span className={styles.label}>
          YÜKLENİYOR
        </span>

        <h1>İçerik hazırlanıyor</h1>

        <p>
          İstediğiniz sayfa hazırlanırken lütfen bekleyin.
        </p>
      </section>
    </main>
  );
}