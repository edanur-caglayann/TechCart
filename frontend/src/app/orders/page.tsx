"use client";

import {
  ArrowLeft,
  CalendarDays,
  ChevronRight,
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

// Sipariş durumlarının Türkçe karşılıkları.
const orderStatusLabels: Record<
  OrderStatus,
  string
> = {
  Preparing: "Hazırlanıyor",
  Shipped: "Kargoya Verildi",
  Delivered: "Teslim Edildi",
};

export default function OrdersPage() {
  const router = useRouter();

  const {
    user,
    isAuthLoading,
  } = useAuth();

  /*
    Giriş yapan kullanıcıya ait siparişlere
    OrderContext üzerinden ulaşırız.
  */
  const {
    orders,
    isOrderLoading,
  } = useOrder();

  // Kullanıcı giriş yapmamışsa giriş ekranına gönderilir.
  useEffect(() => {
    if (!isAuthLoading && !user) {
      router.replace("/login");
    }
  }, [isAuthLoading, user, router]);

  // Sipariş tarihini Türkçe tarih ve saat biçimine dönüştürür.
  function formatOrderDate(date: string) {
    return new Intl.DateTimeFormat("tr-TR", {
      dateStyle: "long",
      timeStyle: "short",
    }).format(new Date(date));
  }

  // Sipariş durumuna uygun CSS sınıfını belirler.
  function getStatusClass(status: OrderStatus) {
    if (status === "Shipped") {
      return styles.shipped;
    }

    if (status === "Delivered") {
      return styles.delivered;
    }

    return styles.preparing;
  }

  // Oturum ve sipariş bilgileri yüklenirken gösterilir.
  if (
    isAuthLoading ||
    isOrderLoading ||
    !user
  ) {
    return (
      <main className={styles.ordersPage}>
        <p className={styles.loadingText}>
          Siparişler yükleniyor...
        </p>
      </main>
    );
  }

  return (
    <main className={styles.ordersPage}>
      <header className={styles.topBar}>
        <Link className={styles.logo} href="/">
          TECHCART
        </Link>

        <Link
          className={styles.backLink}
          href="/"
        >
          <ArrowLeft size={18} />
          Ana sayfaya dön
        </Link>
      </header>

      <section className={styles.ordersContainer}>
        <div className={styles.pageHeader}>
          <span className={styles.label}>
            SİPARİŞ GEÇMİŞİ
          </span>

          <h1>Siparişlerim</h1>

          <p>
            Geçmiş siparişlerini, ürünlerini ve
            sipariş durumlarını görüntüleyebilirsin.
          </p>
        </div>

        {orders.length === 0 ? (
          // Kullanıcının siparişi yoksa boş görünüm gösterilir.
          <section className={styles.emptyState}>
            <ShoppingBag
              size={48}
              strokeWidth={1.3}
            />

            <h2>Henüz siparişin bulunmuyor</h2>

            <p>
              Ürünleri inceleyerek ilk siparişini
              oluşturabilirsin.
            </p>

            <Link href="/#products">
              Ürünleri Keşfet
            </Link>
          </section>
        ) : (
          // Kullanıcının geçmiş siparişleri listelenir.
          <div className={styles.orderList}>
            {orders.map((order) => (
              <article
                className={styles.orderCard}
                key={order.id}
              >
                <div className={styles.orderHeader}>
                  <div>
                    <span>Sipariş numarası</span>

                    <strong>
                      {order.orderNumber}
                    </strong>
                  </div>

                  <span
                    className={`${styles.statusBadge} ${getStatusClass(
                      order.status
                    )}`}
                  >
                    {orderStatusLabels[order.status]}
                  </span>
                </div>

                <div
                  className={styles.orderInformation}
                >
                  <div>
                    <CalendarDays size={18} />

                    <span>
                      {formatOrderDate(
                        order.createdAt
                      )}
                    </span>
                  </div>

                  <div>
                    <ShoppingBag size={18} />

                    <span>
                      {order.totalQuantity} ürün
                    </span>
                  </div>

                  <div>
                    <MapPin size={18} />

                    <span>
                      {order.deliveryAddress.district}
                      {" / "}
                      {order.deliveryAddress.city}
                    </span>
                  </div>
                </div>

                {/* Siparişteki ürünlerin kısa özeti */}
                <div className={styles.productSummary}>
                  {order.items.map((item) => (
                    <div
                      className={styles.productItem}
                      key={item.productId}
                    >
                      <div>
                        <strong>{item.name}</strong>

                        <span>
                          {item.brand} • {item.model}
                        </span>
                      </div>

                      <span>
                        {item.quantity} adet
                      </span>
                    </div>
                  ))}
                </div>

                <div className={styles.orderFooter}>
                  <div>
                    <span>Ödenen toplam</span>

                    <strong>
                      {formatCurrency(
                        order.totalPrice
                      )}
                    </strong>
                  </div>

                  {/*
                    Kullanıcıyı seçilen siparişin
                    ayrıntılarının bulunduğu ekrana gönderir.
                  */}
                  <Link
                    className={styles.detailLink}
                    href={`/orders/success/${order.orderNumber}`}
                  >
                    Siparişi Görüntüle
                    <ChevronRight size={18} />
                  </Link>
                </div>
              </article>
            ))}
          </div>
        )}
      </section>
    </main>
  );
}