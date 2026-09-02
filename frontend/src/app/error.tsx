"use client";

import {
  Home,
  RefreshCw,
  TriangleAlert,
} from "lucide-react";
import Link from "next/link";
import { useEffect } from "react";

import styles from "./error.module.css";

type ErrorPageProps = {
  error: Error & {
    digest?: string;
  };
  reset: () => void;
};

export default function ErrorPage({
  error,
  reset,
}: ErrorPageProps) {
  /*
    Oluşan hatayı inceleyebilmek için konsola yazdiririz
  */
  useEffect(() => {
    console.error(error);
  }, [error]);

  return (
    <main className={styles.errorPage}>
      <section className={styles.errorCard}>
        <div className={styles.iconArea}>
          <TriangleAlert size={45} />
        </div>

        <span className={styles.errorLabel}>
          BEKLENMEYEN HATA
        </span>

        <h1>Bir şeyler ters gitti</h1>

        <p>
          İşlem sırasında beklenmeyen bir sorun
          oluştu. Yeniden deneyebilir veya ana
          sayfaya dönebilirsiniz.
        </p>

        <div className={styles.actions}>
          {/*
            reset fonksiyonu hata oluşan sayfayı
            yeniden çalıştırmayı dener.
          */}
          <button
            className={styles.retryButton}
            type="button"
            onClick={reset}
          >
            <RefreshCw size={18} />
            Yeniden Dene
          </button>

          <Link
            className={styles.homeButton}
            href="/"
          >
            <Home size={18} />
            Ana Sayfaya Dön
          </Link>
        </div>

        {/*
          Next.js tarafından hata kimliği
          oluşturulmuşsa kullanıcıya gösterilir.
        */}
        {error.digest && (
          <span className={styles.errorCode}>
            Hata kodu: {error.digest}
          </span>
        )}
      </section>
    </main>
  );
}