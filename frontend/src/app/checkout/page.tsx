"use client";

import {
  ArrowLeft,
  LockKeyhole,
  MapPin,
  ShieldCheck,
  ShoppingBag,
} from "lucide-react";

import { useEffect, useState } from "react";

import Link from "next/link";

import { useAddress } from "../../context/AddressContext";
import { useCart } from "../../context/CartContext";

import { ApiError } from "../../services/apiClient";
import { createOrderRequest, initiateCheckoutRequest } from "../../services/orderService";

import {
  calculateIncludedVat,
  calculateNetAmount,
  DEFAULT_VAT_RATE,
  formatCurrency,
} from "../../utils/tax";

import styles from "./page.module.css";

export default function CheckoutPage() {
  const { cartItems, totalQuantity, totalPrice } = useCart();
  const { addresses, isAddressLoading } = useAddress();

  const [selectedAddressId, setSelectedAddressId] = useState("");
  const [isProcessing, setIsProcessing] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  /*
    Adresler yüklendiğinde varsayılan adresi, varsayılan yoksa ilk adresi otomatik seçer.
  */
  useEffect(() => {
    if (isAddressLoading || addresses.length === 0) {
      return;
    }

    setSelectedAddressId((currentId) => {
      const selectedAddressExists = addresses.some((address) => address.id === currentId);

      if (selectedAddressExists) {
        return currentId;
      }

      const defaultAddress = addresses.find((address) => address.isDefault);
      return defaultAddress?.id ?? addresses[0].id;
    });
  }, [addresses, isAddressLoading]);

  const totalNetPrice = cartItems.reduce((total, item) => {
    const itemTotal = item.product.price * item.quantity;
    const vatRate = item.product.vatRate ?? DEFAULT_VAT_RATE;
    return total + calculateNetAmount(itemTotal, vatRate);
  }, 0);

  const totalVat = cartItems.reduce((total, item) => {
    const itemTotal = item.product.price * item.quantity;
    const vatRate = item.product.vatRate ?? DEFAULT_VAT_RATE;
    return total + calculateIncludedVat(itemTotal, vatRate);
  }, 0);

  /*
    "Ödemeye Geç" butonuna basılınca: önce siparişi oluştur (stok rezerve
    edilir), sonra iyzico'da ödemeyi başlat, dönen paymentPageUrl'e GERÇEK
    bir yönlendirmeyle (router.push DEĞİL) git — kullanıcı sitemizden çıkıp
    iyzico'nun kendi sayfasına gidiyor.
  */
  async function handleCheckout() {
    if (cartItems.length === 0 || !selectedAddressId) {
      return;
    }

    setErrorMessage("");
    setIsProcessing(true);

    try {
      const order = await createOrderRequest(selectedAddressId);
      const checkout = await initiateCheckoutRequest(order.id);

      window.location.href = checkout.paymentPageUrl;
    } catch (error) {
      if (error instanceof ApiError) {
        setErrorMessage(error.message);
      } else {
        setErrorMessage("Ödeme başlatılamadı. Lütfen tekrar deneyin.");
      }
      setIsProcessing(false);
    }
  }

  return (
    <main className={styles.checkoutPage}>
      <header className={styles.topBar}>
        <Link className={styles.logo} href="/">
          TECHCART
        </Link>

        <Link className={styles.backLink} href="/cart">
          <ArrowLeft size={18} />
          Sepete dön
        </Link>
      </header>

      <section className={styles.checkoutContainer}>
        <div className={styles.pageHeader}>
          <span className={styles.label}>GÜVENLİ ÖDEME</span>
          <h1>Ödeme Bilgileri</h1>
          <p>Teslimat adresini seçerek siparişini tamamlayabilirsin.</p>
        </div>

        {cartItems.length === 0 ? (
          <div className={styles.emptyCart}>
            <ShoppingBag size={48} strokeWidth={1.3} />
            <h2>Ödeme için sepetinde ürün bulunmalı</h2>
            <p>Ürünleri inceleyerek alışveriş sepetine en az bir ürün eklemelisin.</p>
            <Link href="/#products">Ürünleri Keşfet</Link>
          </div>
        ) : (
          <div className={styles.checkoutGrid}>
            <div className={styles.formSections}>
              <section className={styles.formCard}>
                <div className={styles.sectionHeader}>
                  <div className={styles.sectionIcon}>
                    <MapPin size={23} />
                  </div>

                  <div>
                    <h2>Teslimat Adresi</h2>
                    <p>Siparişin teslim edileceği kayıtlı adresi seç.</p>
                  </div>
                </div>

                {isAddressLoading ? (
                  <p className={styles.addressLoading}>Adresler yükleniyor...</p>
                ) : addresses.length === 0 ? (
                  <div className={styles.emptyAddress}>
                    <MapPin size={28} />
                    <h3>Kayıtlı adresin bulunmuyor</h3>
                    <p>Ödemeye devam etmek için bir teslimat adresi eklemelisin.</p>
                    <Link href="/addresses">Yeni Adres Ekle</Link>
                  </div>
                ) : (
                  <div className={styles.addressList}>
                    {addresses.map((address) => (
                      <label
                        className={`${styles.addressOption} ${
                          selectedAddressId === address.id ? styles.selectedAddress : ""
                        }`}
                        key={address.id}
                      >
                        <input
                          type="radio"
                          name="deliveryAddress"
                          value={address.id}
                          checked={selectedAddressId === address.id}
                          onChange={() => setSelectedAddressId(address.id)}
                        />

                        <div className={styles.addressContent}>
                          <div className={styles.addressTitle}>
                            <strong>{address.title}</strong>
                            {address.isDefault && <span className={styles.defaultBadge}>Varsayılan</span>}
                          </div>

                          <strong>
                            {address.firstName} {address.lastName}
                          </strong>

                          <p>
                            {address.neighborhood}, {address.addressLine}
                          </p>

                          <span>
                            {address.district} / {address.city}
                            <br />
                            {address.phone}
                          </span>
                        </div>
                      </label>
                    ))}

                    <Link className={styles.manageAddressLink} href="/addresses">
                      Adresleri Yönet
                    </Link>
                  </div>
                )}
              </section>

              <div className={styles.securityInformation}>
                <ShieldCheck size={21} />
                <p>
                  Kart bilgilerin TechCart tarafından hiç görülmez veya saklanmaz — ödeme, iyzico&apos;nun
                  güvenli sayfası üzerinden tamamlanır.
                </p>
              </div>
            </div>

            <aside className={styles.orderSummary}>
              <h2>Sipariş Özeti</h2>

              <div className={styles.productList}>
                {cartItems.map((item) => (
                  <div className={styles.productItem} key={item.product.id}>
                    <div>
                      <strong>{item.product.name}</strong>
                      <span>{item.quantity} adet</span>
                    </div>

                    <strong>{formatCurrency(item.product.price * item.quantity)}</strong>
                  </div>
                ))}
              </div>

              <div className={styles.summaryRow}>
                <span>Ürün adedi</span>
                <strong>{totalQuantity}</strong>
              </div>

              <div className={styles.summaryRow}>
                <span>KDV hariç ara toplam</span>
                <strong>{formatCurrency(totalNetPrice)}</strong>
              </div>

              <div className={styles.summaryRow}>
                <span>Toplam KDV</span>
                <strong>{formatCurrency(totalVat)}</strong>
              </div>

              <div className={styles.summaryRow}>
                <span>Kargo</span>
                <strong>Ücretsiz</strong>
              </div>

              <div className={styles.totalRow}>
                <span>KDV dahil toplam</span>
                <strong>{formatCurrency(totalPrice)}</strong>
              </div>

              {errorMessage && (
                <p className={styles.errorMessage} role="alert">
                  {errorMessage}
                </p>
              )}

              <button
                className={styles.paymentButton}
                type="button"
                onClick={handleCheckout}
                disabled={isAddressLoading || !selectedAddressId || isProcessing}
              >
                <LockKeyhole size={19} />
                {isProcessing ? "Yönlendiriliyor..." : "Ödemeye Geç"}
              </button>
            </aside>
          </div>
        )}
      </section>
    </main>
  );
}