"use client";

import { MapPin, PackageCheck, ShoppingBag } from "lucide-react";
import Link from "next/link";
import { useParams } from "next/navigation";

import { useOrder } from "../../../../context/OrderContext";
import type { OrderStatus } from "../../../../types/order";
import { formatCurrency } from "../../../../utils/tax";

import styles from "./page.module.css";

const orderStatusLabels: Record<OrderStatus, string> = {
  AwaitingPayment: "Ödeme bekleniyor",
  Paid: "Ödendi",
  Cancelled: "İptal edildi",
};

export default function OrderSuccessPage() {
  const params = useParams<{ orderNumber: string }>();
  const {
    orders,
    isOrderLoading,
    orderError,
    refreshOrders,
  } = useOrder();

  const orderNumber = decodeURIComponent(params.orderNumber);

  const order = orders.find(
    (candidate) => candidate.orderNumber === orderNumber
  );

  if (isOrderLoading) {
    return (
      <main className={styles.successPage}>
        <p className={styles.loadingText}>
          Sipariş bilgileri yükleniyor...
        </p>
      </main>
    );
  }

  if (orderError) {
    return (
      <main className={styles.successPage}>
        <p role="alert">{orderError}</p>

        <button
          type="button"
          onClick={() => {
            void refreshOrders();
          }}
        >
          Tekrar dene
        </button>
      </main>
    );
  }

  if (!order) {
    return (
      <main className={styles.successPage}>
        <section className={styles.notFoundCard}>
          <ShoppingBag size={42} />
          <h1>Sipariş bulunamadı</h1>
          <p>İstenen sipariş bilgisine ulaşılamadı.</p>
          <Link href="/orders">Siparişlerime Git</Link>
        </section>
      </main>
    );
  }

  const totalQuantity = order.items.reduce(
    (total, item) => total + item.quantity,
    0
  );

  const orderDate = new Date(order.createdAt).toLocaleString("tr-TR", {
    dateStyle: "long",
    timeStyle: "short",
  });

  return (
    <main className={styles.successPage}>
      <header className={styles.topBar}>
        <Link className={styles.logo} href="/">
          TECHCART
        </Link>

        <Link className={styles.ordersLink} href="/orders">
          Siparişlerim
        </Link>
      </header>

      <section className={styles.successContainer}>
        <div className={styles.successHeader}>
          <div className={styles.successIcon}>
            <PackageCheck size={42} />
          </div>

          <span className={styles.label}>SİPARİŞ BİLGİLERİ</span>
          <h1>{orderStatusLabels[order.status]}</h1>

          <p>
            Siparişinin güncel durumunu ve detaylarını aşağıda
            görebilirsin.
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
              <strong>{orderStatusLabels[order.status]}</strong>
            </div>
          </div>
        </div>

        <div className={styles.detailGrid}>
          <section className={styles.detailCard}>
            <div className={styles.cardHeader}>
              <PackageCheck size={22} />

              <div>
                <h2>Sipariş Ürünleri</h2>
                <p>Siparişinde bulunan ürünler.</p>
              </div>
            </div>

            <div className={styles.productList}>
              {order.items.map((item, index) => (
                <article
                  className={styles.productItem}
                  key={item.productId ?? `${order.id}-${index}`}
                >
                  <div>
                    <strong>{item.productName}</strong>
                    <span>{item.productModel}</span>
                    <span>{item.quantity} adet</span>
                  </div>

                  <strong>
                    {formatCurrency(item.unitPrice * item.quantity)}
                  </strong>
                </article>
              ))}
            </div>
          </section>

          <section className={styles.detailCard}>
            <div className={styles.cardHeader}>
              <MapPin size={22} />

              <div>
                <h2>Teslimat Adresi</h2>
                <p>Siparişin teslim edileceği adres.</p>
              </div>
            </div>

            <div className={styles.addressInformation}>
              <strong>{order.shippingFullName}</strong>
              <p>{order.shippingAddressLine}</p>

              <span>
                {order.shippingDistrict} / {order.shippingCity}
              </span>

              <span>{order.shippingPostalCode}</span>
              <span>{order.shippingPhone}</span>
            </div>
          </section>
        </div>

        <section className={styles.summaryCard}>
          <h2>Tutar Özeti</h2>

          <div className={styles.summaryRow}>
            <span>Ürün adedi</span>
            <strong>{totalQuantity}</strong>
          </div>

          <div className={styles.summaryRow}>
            <span>KDV hariç ara toplam</span>
            <strong>{formatCurrency(order.subtotal)}</strong>
          </div>

          <div className={styles.summaryRow}>
            <span>Toplam KDV</span>
            <strong>{formatCurrency(order.vatTotal)}</strong>
          </div>

          <div className={styles.summaryRow}>
            <span>Kargo</span>
            <strong>{formatCurrency(order.shippingFee)}</strong>
          </div>

          <div className={styles.totalRow}>
            <span>
              {order.status === "Paid"
                ? "Ödenen toplam"
                : "Sipariş toplamı"}
            </span>

            <strong>{formatCurrency(order.total)}</strong>
          </div>
        </section>

        <div className={styles.actions}>
          <Link className={styles.secondaryButton} href="/">
            Ana Sayfaya Dön
          </Link>

          <Link className={styles.primaryButton} href="/orders">
            Siparişlerimi Görüntüle
          </Link>
        </div>
      </section>
    </main>
  );
}