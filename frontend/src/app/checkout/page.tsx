"use client";

import {
  ArrowLeft,
  CreditCard,
  LockKeyhole,
  MapPin,
  ShieldCheck,
  ShoppingBag,
} from "lucide-react";

import {
  useEffect,
  useState,
} from "react";

import { zodResolver } from "@hookform/resolvers/zod";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";

import { useAddress } from "../../context/AddressContext";
import { useCart } from "../../context/CartContext";

import {
  paymentSchema,
  type PaymentFormValues,
} from "../../schemas/paymentSchemas";

import {
  calculateIncludedVat,
  calculateNetAmount,
  DEFAULT_VAT_RATE,
  formatCurrency,
} from "../../utils/tax";

import styles from "./page.module.css";

export default function CheckoutPage() {
  /*
    useRouter -> ödeme doğrulaması tamamlandığında
    kullanıcıyı 3D Secure ekranına yönlendirir.
  */
  const router = useRouter();

  /*
    Sipariş özetini oluşturmak için sepet bilgilerine ulaşıyoruz.
  */
  const {
    cartItems,
    totalQuantity,
    totalPrice,
  } = useCart();

  /*
    Kullanıcının kayıtlı teslimat adreslerine AddressContext üzerinden ulaşıyoruz.
  */
  const {
    addresses,
    isAddressLoading,
  } = useAddress();

  // Ödeme için seçilen teslimat adresini tutar.
  const [
    selectedAddressId,
    setSelectedAddressId,
  ] = useState("");

  /*
    React Hook Form kart alanlarını yönetir.
    zodResolver girilen değerleri paymentSchema
    kurallarıyla doğrular.
  */
  const {
    register,
    handleSubmit,
    formState: {
      errors,
      isSubmitting,
    },
  } = useForm<PaymentFormValues>({
    resolver: zodResolver(paymentSchema),
    defaultValues: {
      cardHolder: "",
      cardNumber: "",
      expiryDate: "",
      cvv: "",
      useThreeDSecure: false,
    },
    mode: "onBlur",
  });

  /*
    Adresler yüklendiğinde varsayılan adresi, varsayılan yoksa ilk adresi otomatik seçer.
  */
  useEffect(() => {
    if (
      isAddressLoading ||
      addresses.length === 0
    ) {
      return;
    }

    setSelectedAddressId((currentId) => {
      const selectedAddressExists =
        addresses.some(
          (address) =>
            address.id === currentId
        );

      if (selectedAddressExists) {
        return currentId;
      }

      const defaultAddress = addresses.find(
        (address) => address.isDefault
      );

      return (
        defaultAddress?.id ??
        addresses[0].id
      );
    });
  }, [addresses, isAddressLoading]);

  /*
    Sepetteki ürünlerin KDV hariç toplam tutarını hesaplar.
  */
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

  /*
    Sepetteki ürün fiyatlarına dahil olan toplam KDV tutarını hesaplar.
  */
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
    Kullanıcı formdaki butona bastığında çalışan fonksiyon.
  */
  /*
    Kart bilgileri Zod doğrulamasından başarıyla
    geçtiğinde çalışan ödeme fonksiyonudur.
  */
  function onSubmit() {
    /*
      Sepet boşsa ödeme işlemine devam edilmez.
    */
    if (cartItems.length === 0) {
      return;
    }

    /*
      Teslimat adresi seçilmediyse
      ödeme işlemine devam edilmez.
    */
    if (!selectedAddressId) {
      return;
    }

    /*
      Seçilen adresin kimliğini 3D Secure
      işleminde kullanmak üzere geçici olarak saklarız.
    */
    sessionStorage.setItem(
      "techcart-checkout-address-id",
      selectedAddressId
    );

    /*
      Kart bilgileri doğrulandıktan sonra kullanıcı
      3D Secure ekranına yönlendirilir.
    */
    router.push("/checkout/3d-secure");
  }

  return (
    <main className={styles.checkoutPage}>
      {/*
        Sayfanın üst kısmındaki logo ve
        sepete dönüş bağlantısı.
      */}
      <header className={styles.topBar}>
        <Link className={styles.logo} href="/">
          TECHCART
        </Link>

        <Link
          className={styles.backLink}
          href="/cart"
        >
          <ArrowLeft size={18} />
          Sepete dön
        </Link>
      </header>

      <section
        className={styles.checkoutContainer}
      >
        <div className={styles.pageHeader}>
          <span className={styles.label}>
            GÜVENLİ ÖDEME
          </span>

          <h1>Ödeme Bilgileri</h1>

          <p>
            Teslimat adresini seçip kart bilgilerini
            girerek siparişini tamamlayabilirsin.
          </p>
        </div>

        {/*
          Sepet boşsa ödeme formu yerine
          kullanıcıya bilgilendirme gösterilir.
        */}
        {cartItems.length === 0 ? (
          <div className={styles.emptyCart}>
            <ShoppingBag
              size={48}
              strokeWidth={1.3}
            />

            <h2>
              Ödeme için sepetinde ürün bulunmalı
            </h2>

            <p>
              Ürünleri inceleyerek alışveriş
              sepetine en az bir ürün eklemelisin.
            </p>

            <Link href="/#products">
              Ürünleri Keşfet
            </Link>
          </div>
        ) : (
          /*
            HTML form kontrolleri yapılır.
            Kullanıcı gerekli bilgileri girmeden
            ödeme işlemine devam edemez.
          */
          <form
            className={styles.checkoutGrid}
            onSubmit={handleSubmit(onSubmit)}
            noValidate
          >
            <div className={styles.formSections}>
              {/*
                Teslimat adreslerinin bulunduğu bölüm.
              */}
              <section className={styles.formCard}>
                <div
                  className={styles.sectionHeader}
                >
                  <div
                    className={styles.sectionIcon}
                  >
                    <MapPin size={23} />
                  </div>

                  <div>
                    <h2>Teslimat Adresi</h2>

                    <p>
                      Siparişin teslim edileceği
                      kayıtlı adresi seç.
                    </p>
                  </div>
                </div>

                {isAddressLoading ? (
                  <p
                    className={
                      styles.addressLoading
                    }
                  >
                    Adresler yükleniyor...
                  </p>
                ) : addresses.length === 0 ? (
                  <div
                    className={styles.emptyAddress}
                  >
                    <MapPin size={28} />

                    <h3>
                      Kayıtlı adresin bulunmuyor
                    </h3>

                    <p>
                      Ödemeye devam etmek için bir
                      teslimat adresi eklemelisin.
                    </p>

                    <Link href="/addresses">
                      Yeni Adres Ekle
                    </Link>
                  </div>
                ) : (
                  <div
                    className={styles.addressList}
                  >
                    {addresses.map((address) => (
                      <label
                        className={`${styles.addressOption} ${
                          selectedAddressId ===
                          address.id
                            ? styles.selectedAddress
                            : ""
                        }`}
                        key={address.id}
                      >
                        <input
                          type="radio"
                          name="deliveryAddress"
                          value={address.id}
                          checked={
                            selectedAddressId ===
                            address.id
                          }
                          onChange={() =>
                            setSelectedAddressId(
                              address.id
                            )
                          }
                        />

                        <div
                          className={
                            styles.addressContent
                          }
                        >
                          <div
                            className={
                              styles.addressTitle
                            }
                          >
                            <strong>
                              {address.title}
                            </strong>

                            {address.isDefault && (
                              <span
                                className={
                                  styles.defaultBadge
                                }
                              >
                                Varsayılan
                              </span>
                            )}
                          </div>

                          <strong>
                            {address.firstName}{" "}
                            {address.lastName}
                          </strong>

                          <p>
                            {address.neighborhood},{" "}
                            {address.addressLine}
                          </p>

                          <span>
                            {address.district} /{" "}
                            {address.city}
                            <br />
                            {address.phone}
                          </span>
                        </div>
                      </label>
                    ))}

                    <Link
                      className={
                        styles.manageAddressLink
                      }
                      href="/addresses"
                    >
                      Adresleri Yönet
                    </Link>
                  </div>
                )}
              </section>

              {/*
                Kart bilgilerinin bulunduğu bölüm.
                Bu bilgiler localStorage içerisine kaydedilmez.
              */}
              <section className={styles.formCard}>
                <div
                  className={styles.sectionHeader}
                >
                  <div
                    className={styles.sectionIcon}
                  >
                    <CreditCard size={23} />
                  </div>

                  <div>
                    <h2>Kart Bilgileri</h2>

                    <p>
                      Ödeme için kart bilgilerini gir.
                    </p>
                  </div>
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="cardHolder">
                    Kart üzerindeki ad soyad
                  </label>

                  <input
                    id="cardHolder"
                    type="text"
                    placeholder="AD SOYAD"
                    autoComplete="cc-name"
                    aria-invalid={Boolean(
                      errors.cardHolder
                    )}
                    className={
                      errors.cardHolder
                        ? styles.inputError
                        : ""
                    }
                    {...register("cardHolder")}
                  />

                  {errors.cardHolder && (
                    <p
                      className={
                        styles.errorMessage
                      }
                      role="alert"
                    >
                      {errors.cardHolder.message}
                    </p>
                  )}
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="cardNumber">
                    Kart numarası
                  </label>

                  <div
                    className={styles.secureInput}
                  >
                    <CreditCard
                      size={19}
                      aria-hidden="true"
                    />

                    <input
                      id="cardNumber"
                      type="text"
                      inputMode="numeric"
                      placeholder="0000 0000 0000 0000"
                      autoComplete="cc-number"
                      maxLength={19}
                      aria-invalid={Boolean(
                        errors.cardNumber
                      )}
                      className={
                        errors.cardNumber
                          ? styles.inputError
                          : ""
                      }
                      {...register("cardNumber")}
                    />
                  </div>

                  {errors.cardNumber && (
                    <p
                      className={
                        styles.errorMessage
                      }
                      role="alert"
                    >
                      {errors.cardNumber.message}
                    </p>
                  )}
                </div>

                <div className={styles.formRow}>
                  <div
                    className={styles.formGroup}
                  >
                    <label htmlFor="expiryDate">
                      Son kullanma tarihi
                    </label>

                    <input
                      id="expiryDate"
                      type="text"
                      inputMode="numeric"
                      placeholder="AA/YY"
                      autoComplete="cc-exp"
                      maxLength={5}
                      aria-invalid={Boolean(
                        errors.expiryDate
                      )}
                      className={
                        errors.expiryDate
                          ? styles.inputError
                          : ""
                      }
                      {...register("expiryDate")}
                    />

                    {errors.expiryDate && (
                      <p
                        className={
                          styles.errorMessage
                        }
                        role="alert"
                      >
                        {errors.expiryDate.message}
                      </p>
                    )}
                  </div>

                  <div
                    className={styles.formGroup}
                  >
                    <label htmlFor="cvv">
                      CVV
                    </label>

                    <div
                      className={styles.secureInput}
                    >
                      <LockKeyhole
                        size={18}
                        aria-hidden="true"
                      />

                      <input
                        id="cvv"
                        type="password"
                        inputMode="numeric"
                        placeholder="123"
                        autoComplete="cc-csc"
                        maxLength={4}
                        aria-invalid={Boolean(
                          errors.cvv
                        )}
                        className={
                          errors.cvv
                            ? styles.inputError
                            : ""
                        }
                        {...register("cvv")}
                      />
                    </div>

                    {errors.cvv && (
                      <p
                        className={
                          styles.errorMessage
                        }
                        role="alert"
                      >
                        {errors.cvv.message}
                      </p>
                    )}
                  </div>
                </div>

                <label
                  className={
                    styles.secureAgreement
                  }
                >
                  <input
                    type="checkbox"
                    {...register(
                      "useThreeDSecure"
                    )}
                  />

                  <span>
                    Ödemenin 3D Secure
                    doğrulamasıyla tamamlanmasını
                    kabul ediyorum.
                  </span>
                </label>

                {errors.useThreeDSecure && (
                  <p
                    className={
                      styles.errorMessage
                    }
                    role="alert"
                  >
                    {
                      errors.useThreeDSecure
                        .message
                    }
                  </p>
                )}

                <div
                  className={
                    styles.securityInformation
                  }
                >
                  <ShieldCheck size={21} />

                  <p>
                    Kart bilgilerin TechCart
                    tarafından saklanmaz. Ödeme
                    işlemi güvenli ödeme altyapısı
                    üzerinden yürütülecektir.
                  </p>
                </div>
              </section>
            </div>

            {/*
              Ana ödeme formunu destekleyen
              sipariş özeti yan paneli.
            */}
            <aside className={styles.orderSummary}>
              <h2>Sipariş Özeti</h2>

              <div className={styles.productList}>
                {cartItems.map((item) => (
                  <div
                    className={styles.productItem}
                    key={item.product.id}
                  >
                    <div>
                      <strong>
                        {item.product.name}
                      </strong>

                      <span>
                        {item.quantity} adet
                      </span>
                    </div>

                    <strong>
                      {formatCurrency(
                        item.product.price *
                          item.quantity
                      )}
                    </strong>
                  </div>
                ))}
              </div>

              <div className={styles.summaryRow}>
                <span>Ürün adedi</span>
                <strong>{totalQuantity}</strong>
              </div>

              <div className={styles.summaryRow}>
                <span>
                  KDV hariç ara toplam
                </span>

                <strong>
                  {formatCurrency(totalNetPrice)}
                </strong>
              </div>

              <div className={styles.summaryRow}>
                <span>Toplam KDV</span>

                <strong>
                  {formatCurrency(totalVat)}
                </strong>
              </div>

              <div className={styles.summaryRow}>
                <span>Kargo</span>
                <strong>Ücretsiz</strong>
              </div>

              <div className={styles.totalRow}>
                <span>KDV dahil toplam</span>

                <strong>
                  {formatCurrency(totalPrice)}
                </strong>
              </div>

              {/*
                Form başarılı şekilde doğrulanırsa
                handleSubmit fonksiyonu çalışır.
              */}
              <button
                className={styles.paymentButton}
                type="submit"
                disabled={
                  isAddressLoading ||
                  !selectedAddressId ||
                  isSubmitting
                }
              >
                <LockKeyhole size={19} />

                {isSubmitting
                  ? "Ödeme hazırlanıyor..."
                  : "3D Secure ile Ödemeye Geç"}
              </button>
            </aside>
          </form>
        )}
      </section>
    </main>
  );
}