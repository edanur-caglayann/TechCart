"use client";

import {
  CircleCheck,
  CircleX,
  LoaderCircle,
} from "lucide-react";

import Link from "next/link";
import { useSearchParams } from "next/navigation";

import styles from "./page.module.css";

/*
  Bu sayfaya artık kullanıcı DOĞRUDAN gelmiyor — iyzico'nun ödeme sayfasında
  işlem tamamlandıktan sonra, backend'in callback endpoint'i (iyzico'dan
  gelen sonucu işleyip) tarayıcıyı buraya, ?status=success veya
  ?status=failed ile yönlendiriyor. Bu sayfa hiçbir API isteği atmıyor,
  hiçbir doğrulama formu göstermiyor — sadece backend'in zaten verdiği
  sonucu okuyup gösteriyor.
*/
export default function PaymentResultPage() {
  const searchParams = useSearchParams();
  const status = searchParams.get("status");

  if (status === "success") {
    return (
      <main className={styles.securePage}>
        <section className={styles.secureContainer}>
          <div className={styles.secureCard}>
            <div className={styles.securityIcon}>
              <CircleCheck size={30} color="#16a34a" />
            </div>

            <span className={styles.label}>ÖDEME BAŞARILI</span>

            <h1>Ödemen alındı</h1>

            <p className={styles.description}>
              Siparişin başarıyla oluşturuldu. Sipariş detaylarını yakında sipariş geçmişinden görebileceksin.
            </p>

            <div className={styles.actions}>
              <Link href="/">Alışverişe devam et</Link>
              <Link href="/orders">Siparişlerimi Görüntüle</Link>
            </div>
          </div>
        </section>
      </main>
    );
  }

  if (status === "failed") {
    return (
      <main className={styles.securePage}>
        <section className={styles.secureContainer}>
          <div className={styles.secureCard}>
            <div className={styles.securityIcon}>
              <CircleX size={30} color="#dc2626" />
            </div>

            <span className={styles.label}>ÖDEME BAŞARISIZ</span>

            <h1>Ödeme tamamlanamadı</h1>

            <p className={styles.description}>
              Kartınla ilgili bir sorun oluştu. Sepetin ve siparişin hâlâ duruyor, dilersen farklı bir kartla
              tekrar deneyebilirsin.
            </p>

            <Link href="/checkout">Tekrar dene</Link>
          </div>
        </section>
      </main>
    );
  }

  /*
    status hiç yoksa (kullanıcı bu sayfaya doğrudan, backend'den
    yönlendirilmeden geldiyse) anlamlı bir sonuç gösteremeyiz.
  */
  return (
    <main className={styles.securePage}>
      <section className={styles.secureContainer}>
        <div className={styles.secureCard}>
          <div className={styles.securityIcon}>
            <LoaderCircle size={30} />
          </div>

          <span className={styles.label}>BULUNAMADI</span>

          <h1>Ödeme sonucu bulunamadı</h1>

          <p className={styles.description}>
            Bu sayfaya doğrudan erişilemez. Ödeme akışını sepetinden başlatmalısın.
          </p>

          <Link href="/cart">Sepete dön</Link>
        </div>
      </section>
    </main>
  );
}