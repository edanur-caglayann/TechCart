"use client";

import {
  CheckCircle2,
  MapPin,
  PackageCheck,
  ShoppingBag,
} from "lucide-react";

import Link from "next/link";
import { useParams } from "next/navigation";

import { useOrder } from "../../../../context/OrderContext";
import type { OrderStatus } from "../../../../types/order";
import { formatCurrency } from "../../../../utils/tax";

import styles from "./page.module.css";

// Sipariş durumlarının ekranda gösterilecek karşılıkları.
const orderStatusLabels: Record<
  OrderStatus,
  string
> = {
  Preparing: "Hazırlanıyor",
  Shipped: "Kargoya Verildi",
  Delivered: "Teslim Edildi",
};

export default function OrderSuccessPage() {
  /*
    Dinamik URL içerisindeki sipariş numarasını alırız.
    Örneğin: /orders/success/TC-12345678-1234
  */
  const params = useParams<{
    orderNumber: string;
  }>();

  const {
    getOrderByNumber,
    isOrderLoading,
  } = useOrder();

  // URL'den gelen sipariş numarasını çözümler.
  const orderNumber = decodeURIComponent(
    params.orderNumber
  );

  // Sipariş numarasına ait siparişi bulur.
  const order =
    getOrderByNumber(orderNumber);

  // Siparişler localStorage üzerinden yüklenirken gösterilir.
  if (isOrderLoading) {
    return (
      <main className={styles.successPage}>
        <p className={styles.loadingText}>
          Sipariş bilgileri yükleniyor...
        </p>
      </main>
    );
  }

  // Sipariş bulunamazsa kullanıcıya bilgilendirme gösterilir.
  if (!order) {
    return (
      <main className={styles.successPage}>
        <section className={styles.notFoundCard}>
          <ShoppingBag size={42} />

          <h1>Sipariş bulunamadı</h1>

          <p>
            İstenen sipariş bilgisine
            ulaşılamadı.
          </p>

          <Link href="/orders">
            Siparişlerime Git
          </Link>
        </section>
      </main>
    );
  }

  const orderDate = new Date(
    order.createdAt
  ).toLocaleString("tr-TR", {
    dateStyle: "long",
    timeStyle: "short",
  });

  return (
    <main className={styles.successPage}>
      <header className={styles.topBar}>
        <Link className={styles.logo} href="/">
          TECHCART
        </Link>

        <Link
          className={styles.ordersLink}
          href="/orders"
        >
          Siparişlerim
        </Link>
      </header>

      <section className={styles.successContainer}>
        <div className={styles.successHeader}>
          <div className={styles.successIcon}>
            <CheckCircle2 size={42} />
          </div>

          <span className={styles.label}>
            SİPARİŞ ONAYI
          </span>

          <h1>Siparişiniz başarıyla alındı</h1>

          <p>
            Siparişiniz hazırlanmak üzere sisteme
            kaydedildi.
          </p>

          <div className={styles.orderInformation}>
            <div>
              <span>Sipariş numarası</span>
              <strong>{order.orderNumber}</strong>
            </div>

            <div>
              <span>Sipariş tarihi</span>
              <strong>{orderDate}</strong>
            </div>

            <div>
              <span>Sipariş durumu</span>
              <strong>
                {orderStatusLabels[order.status]}
              </strong>
            </div>
          </div>
        </div>

        <div className={styles.detailGrid}>
          {/* Siparişte bulunan ürünler */}
          <section className={styles.detailCard}>
            <div className={styles.cardHeader}>
              <PackageCheck size={22} />

              <div>
                <h2>Sipariş Ürünleri</h2>
                <p>
                  Siparişinizde bulunan ürünler.
                </p>
              </div>
            </div>

            <div className={styles.productList}>
              {order.items.map((item) => (
                <article
                  className={styles.productItem}
                  key={item.productId}
                >
                  <div>
                    <strong>{item.name}</strong>

                    <span>
                      {item.brand} • {item.model}
                    </span>

                    <span>
                      {item.quantity} adet
                    </span>
                  </div>

                  <strong>
                    {formatCurrency(
                      item.totalPrice
                    )}
                  </strong>
                </article>
              ))}
            </div>
          </section>

          {/* Siparişte kullanılan teslimat adresi */}
          <section className={styles.detailCard}>
            <div className={styles.cardHeader}>
              <MapPin size={22} />

              <div>
                <h2>Teslimat Adresi</h2>
                <p>
                  Siparişin teslim edileceği adres.
                </p>
              </div>
            </div>

            <div className={styles.addressInformation}>
              <strong>
                {order.deliveryAddress.title}
              </strong>

              <span>
                {order.deliveryAddress.firstName}{" "}
                {order.deliveryAddress.lastName}
              </span>

              <p>
                {
                  order.deliveryAddress
                    .neighborhood
                }
                ,{" "}
                {
                  order.deliveryAddress
                    .addressLine
                }
              </p>

              <span>
                {order.deliveryAddress.district} /{" "}
                {order.deliveryAddress.city}
              </span>

              <span>
                {order.deliveryAddress.phone}
              </span>
            </div>
          </section>
        </div>

        {/* Siparişin nihai fiyat özeti */}
        <section className={styles.summaryCard}>
          <h2>Ödeme Özeti</h2>

          <div className={styles.summaryRow}>
            <span>Ürün adedi</span>
            <strong>{order.totalQuantity}</strong>
          </div>

          <div className={styles.summaryRow}>
            <span>KDV hariç ara toplam</span>
            <strong>
              {formatCurrency(
                order.totalNetPrice
              )}
            </strong>
          </div>

          <div className={styles.summaryRow}>
            <span>Toplam KDV</span>
            <strong>
              {formatCurrency(order.totalVat)}
            </strong>
          </div>

          <div className={styles.summaryRow}>
            <span>Kargo</span>
            <strong>Ücretsiz</strong>
          </div>

          <div className={styles.totalRow}>
            <span>Ödenen toplam</span>
            <strong>
              {formatCurrency(order.totalPrice)}
            </strong>
          </div>
        </section>

        <div className={styles.actions}>
          <Link
            className={styles.secondaryButton}
            href="/"
          >
            Ana Sayfaya Dön
          </Link>

          <Link
            className={styles.primaryButton}
            href="/orders"
          >
            Siparişlerimi Görüntüle
          </Link>
        </div>
      </section>
    </main>
  );
}