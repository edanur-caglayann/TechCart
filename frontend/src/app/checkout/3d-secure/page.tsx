"use client";

import {
  ArrowLeft,
  LoaderCircle,
  LockKeyhole,
  ShieldCheck,
} from "lucide-react";

import Link from "next/link";
import { useRouter } from "next/navigation";
import {
  useEffect,
  useState,
  type FormEvent,
} from "react";

import { useAddress } from "../../../context/AddressContext";
import { useCart } from "../../../context/CartContext";
import { useOrder } from "../../../context/OrderContext";

import {
  calculateIncludedVat,
  calculateNetAmount,
  DEFAULT_VAT_RATE,
  formatCurrency,
} from "../../../utils/tax";

import styles from "./page.module.css";

export default function ThreeDSecurePage() {
  const router = useRouter();

  const {
    cartItems,
    totalQuantity,
    totalPrice,
    clearCart,
  } = useCart();

  const {
    addresses,
    isAddressLoading,
  } = useAddress();

  const { createOrder } = useOrder();

  // Kullanıcının girdiği doğrulama kodunu tutar.
  const [
    verificationCode,
    setVerificationCode,
  ] = useState("");

  // Seçilen teslimat adresinin kimliğini tutar.
  const [
    selectedAddressId,
    setSelectedAddressId,
  ] = useState<string | null>(null);

  const [
    isSelectionLoaded,
    setIsSelectionLoaded,
  ] = useState(false);

  const [isProcessing, setIsProcessing] =
    useState(false);

  const [errorMessage, setErrorMessage] =
    useState("");

  /*
    Ödeme ekranında seçilen teslimat adresini sessionStorage üzerinden okur.
  */
  useEffect(() => {
    const savedAddressId = sessionStorage.getItem(
      "techcart-checkout-address-id"
    );

    setSelectedAddressId(savedAddressId);
    setIsSelectionLoaded(true);
  }, []);

  // Seçilen teslimat adresini adres listesinde bulur.
  const selectedAddress = addresses.find(
    (address) =>
      address.id === selectedAddressId
  );

  // Siparişin KDV hariç toplamını hesaplar.
  const totalNetPrice = cartItems.reduce(
    (total, item) => {
      const itemTotal =
        item.product.price * item.quantity;

      const vatRate =
        item.product.vatRate ??
        DEFAULT_VAT_RATE;

      return (
        total +
        calculateNetAmount(
          itemTotal,
          vatRate
        )
      );
    },
    0
  );

  // Siparişin toplam KDV tutarını hesaplar.
  const totalVat = cartItems.reduce(
    (total, item) => {
      const itemTotal =
        item.product.price * item.quantity;

      const vatRate =
        item.product.vatRate ??
        DEFAULT_VAT_RATE;

      return (
        total +
        calculateIncludedVat(
          itemTotal,
          vatRate
        )
      );
    },
    0
  );

  /*
    Test doğrulama kodu başarılıysa sipariş oluşturulur ve sepet temizlenir.
  */
  function handleVerification(
    event: FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();
    setErrorMessage("");

    if (
      !selectedAddress ||
      cartItems.length === 0
    ) {
      setErrorMessage(
        "Sipariş bilgilerine ulaşılamadı."
      );
      return;
    }

    setIsProcessing(true);

    /*
      Frontend testi için 123456 kodunu başarılı doğrulama kodu olarak kabul ederiz.
    */
    if (verificationCode !== "123456") {
      setErrorMessage(
        "Doğrulama kodu hatalı. Lütfen tekrar deneyin."
      );
      setIsProcessing(false);
      return;
    }

    const newOrder = createOrder({
      items: cartItems.map((item) => ({
        productId: item.product.id,
        name: item.product.name,
        model: item.product.model,
        brand: item.product.brand,
        category: item.product.category,
        quantity: item.quantity,
        unitPrice: item.product.price,
        vatRate:
          item.product.vatRate ??
          DEFAULT_VAT_RATE,
        totalPrice:
          item.product.price *
          item.quantity,
      })),
      deliveryAddress: {
        ...selectedAddress,
      },
      totalQuantity,
      totalNetPrice,
      totalVat,
      totalPrice,
    });

    clearCart();

    sessionStorage.removeItem(
      "techcart-checkout-address-id"
    );

    router.push(
      `/orders/success/${newOrder.orderNumber}`
    );
  }

  if (
    isAddressLoading ||
    !isSelectionLoaded
  ) {
    return (
      <main className={styles.securePage}>
        <div className={styles.loadingState}>
          <LoaderCircle size={32} />
          <p>Doğrulama bilgileri yükleniyor...</p>
        </div>
      </main>
    );
  }

  if (
    !selectedAddress ||
    cartItems.length === 0
  ) {
    return (
      <main className={styles.securePage}>
        <div className={styles.invalidState}>
          <ShieldCheck size={42} />

          <h1>Doğrulama başlatılamadı</h1>

          <p>
            Sepet veya teslimat adresi bilgilerine
            ulaşılamadı.
          </p>

          <Link href="/checkout">
            Ödeme ekranına dön
          </Link>
        </div>
      </main>
    );
  }

  return (
    <main className={styles.securePage}>
      <header className={styles.topBar}>
        <Link className={styles.logo} href="/">
          TECHCART
        </Link>

        <Link
          className={styles.backLink}
          href="/checkout"
        >
          <ArrowLeft size={18} />
          Ödeme ekranına dön
        </Link>
      </header>

      <section className={styles.secureContainer}>
        <div className={styles.secureCard}>
          <div className={styles.securityIcon}>
            <LockKeyhole size={30} />
          </div>

          <span className={styles.label}>
            3D SECURE
          </span>

          <h1>Ödeme doğrulaması</h1>

          <p className={styles.description}>
            Bankanız tarafından gönderilen altı
            haneli doğrulama kodunu girin.
          </p>

          <div className={styles.paymentSummary}>
            <div>
              <span>Ödenecek tutar</span>
              <strong>
                {formatCurrency(totalPrice)}
              </strong>
            </div>

            <div>
              <span>Ürün adedi</span>
              <strong>{totalQuantity}</strong>
            </div>
          </div>

          <form
            className={styles.verificationForm}
            onSubmit={handleVerification}
          >
            <label htmlFor="verificationCode">
              Doğrulama kodu
            </label>

            <input
              id="verificationCode"
              type="text"
              inputMode="numeric"
              autoComplete="one-time-code"
              placeholder="000000"
              maxLength={6}
              value={verificationCode}
              onChange={(event) =>
                setVerificationCode(
                  event.target.value
                    .replace(/\D/g, "")
                    .slice(0, 6)
                )
              }
              required
            />

            <span className={styles.testInformation}>
              Test doğrulama kodu: 123456
            </span>

            {errorMessage && (
              <p
                className={styles.errorMessage}
                role="alert"
              >
                {errorMessage}
              </p>
            )}

            <button
              type="submit"
              disabled={
                verificationCode.length !== 6 ||
                isProcessing
              }
            >
              {isProcessing ? (
                <>
                  <LoaderCircle size={19} />
                  Doğrulanıyor...
                </>
              ) : (
                <>
                  <LockKeyhole size={19} />
                  Ödemeyi Doğrula
                </>
              )}
            </button>
          </form>

          <div className={styles.securityInformation}>
            <ShieldCheck size={20} />

            <p>
              Bu ekran frontend ödeme akışını test
              etmek amacıyla hazırlanmıştır.
            </p>
          </div>
        </div>
      </section>
    </main>
  );
}