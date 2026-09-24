"use client";

import {
  ArrowLeft,
  CalendarDays,
  CreditCard,
  MapPin,
  ShoppingBag,
} from "lucide-react";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect } from "react";

import { useAuth } from "../../context/AuthContext";
import { useOrder } from "../../context/OrderContext";
import type { OrderStatus } from "../../types/order";
import { formatCurrency } from "../../utils/tax";

import styles from "./page.module.css";

const orderStatusLabels: Record<OrderStatus, string> = {
  AwaitingPayment: "Ödeme Bekleniyor",
  Paid: "Ödendi",
  Cancelled: "İptal Edildi",
};

export default function OrdersPage() {
  const router = useRouter();
  const { user, isAuthLoading } = useAuth();
  const { orders, isOrderLoading } = useOrder();

  useEffect(() => {
    if (!isAuthLoading && !user) {
      router.replace("/login");
    }
  }, [isAuthLoading, user, router]);

  function formatOrderDate(date: string) {
    return new Intl.DateTimeFormat("tr-TR", {
      dateStyle: "long",
      timeStyle: "short",
    }).format(new Date(date));
  }

  /*
    Eski tasarımda 3 durum (Preparing/Shipped/Delivered) için 3 renk sınıfı
    vardı. Bizim 3 yeni durumumuzu (AwaitingPayment/Paid/Cancelled) bu
    mevcut sınıflara eşliyoruz.
  */
  function getStatusClass(status: OrderStatus) {
    if (status === "Paid") return styles.delivered;
    if (status === "Cancelled") return styles.shipped;
    return styles.preparing;
  }

  if (isAuthLoading || isOrderLoading || !user) {
    return (
      <main className={styles.ordersPage}>
        <p className={styles.loadingText}>Siparişler yükleniyor...</p>
      </main>
    );
  }

  return (
    <main className={styles.ordersPage}>
      <header className={styles.topBar}>
        <Link className={styles.logo} href="/">
          TECHCART
        </Link>

        <Link className={styles.backLink} href="/">
          <ArrowLeft size={18} />
          Ana sayfaya dön
        </Link>
      </header>

      <section className={styles.ordersContainer}>
        <div className={styles.pageHeader}>
          <span className={styles.label}>SİPARİŞ GEÇMİŞİ</span>
          <h1>Siparişlerim</h1>
          <p>Geçmiş siparişlerini, ürünlerini ve sipariş durumlarını görüntüleyebilirsin.</p>
        </div>

        {orders.length === 0 ? (
          <section className={styles.emptyState}>
            <ShoppingBag size={48} strokeWidth={1.3} />
            <h2>Henüz siparişin bulunmuyor</h2>
            <p>Ürünleri inceleyerek ilk siparişini oluşturabilirsin.</p>
            <Link href="/#products">Ürünleri Keşfet</Link>
          </section>
        ) : (
          <div className={styles.orderList}>
            {orders.map((order) => {
              const totalQuantity = order.items.reduce((sum, item) => sum + item.quantity, 0);

              return (
                <article className={styles.orderCard} key={order.id}>
                  <div className={styles.orderHeader}>
                    <div>
                      <span>Sipariş numarası</span>
                      <strong>{order.orderNumber}</strong>
                    </div>

                    <span className={`${styles.statusBadge} ${getStatusClass(order.status)}`}>
                      {orderStatusLabels[order.status]}
                    </span>
                  </div>

                  <div className={styles.orderInformation}>
                    <div>
                      <CalendarDays size={18} />
                      <span>{formatOrderDate(order.createdAt)}</span>
                    </div>

                    <div>
                      <ShoppingBag size={18} />
                      <span>{totalQuantity} ürün</span>
                    </div>

                    <div>
                      <MapPin size={18} />
                      <span>
                        {order.shippingDistrict} / {order.shippingCity}
                      </span>
                    </div>
                  </div>

                  <div className={styles.productSummary}>
                    {order.items.map((item, index) => (
                      <div className={styles.productItem} key={item.productId ?? index}>
                        <div>
                          <strong>{item.productName}</strong>
                          <span>{item.productModel}</span>
                        </div>

                        <span>
                          {item.quantity} adet · {formatCurrency(item.unitPrice * item.quantity)}
                        </span>
                      </div>
                    ))}
                  </div>

                  <div className={styles.detailSection}>
                    <MapPin size={18} />
                    <div>
                      <strong>{order.shippingFullName}</strong>
                      <span>{order.shippingAddressLine}</span>
                      <span>
                        {order.shippingDistrict} / {order.shippingCity} — {order.shippingPostalCode}
                      </span>
                      <span>{order.shippingPhone}</span>
                    </div>
                  </div>

                  {order.payment && (
                    <div className={styles.detailSection}>
                      <CreditCard size={18} />
                      <div>
                        <strong>{order.payment.provider}</strong>
                        {order.payment.maskedCardNumber && <span>{order.payment.maskedCardNumber}</span>}
                        <span>{formatOrderDate(order.payment.paidAt)}</span>
                      </div>
                    </div>
                  )}

                  <div className={styles.priceBreakdown}>
                    <div className={styles.priceBreakdownRow}>
                      <span>KDV hariç ara toplam</span>
                      <span>{formatCurrency(order.subtotal)}</span>
                    </div>

                    <div className={styles.priceBreakdownRow}>
                      <span>Toplam KDV</span>
                      <span>{formatCurrency(order.vatTotal)}</span>
                    </div>
                  </div>

                  <div className={styles.orderFooter}>
                    <div>
                      <span>Ödenen toplam</span>
                      <strong>{formatCurrency(order.total)}</strong>
                    </div>
                  </div>
                </article>
              );
            })}
          </div>
        )}
      </section>
    </main>
  );
}