"use client";

import {
  ArrowLeft,
  CreditCard,
  LockKeyhole,
  MapPin,
  ShieldCheck,
  ShoppingBag,
} from "lucide-react";

import type { FormEvent } from "react";

import Link from "next/link";
import { useRouter } from "next/navigation";

import { useCart } from "../../context/CartContext";
import styles from "./page.module.css";

export default function CheckoutPage() {
  /*
    useRouter -> mevcut sepepetteki urun bilgileirni,
    adeti, fiyati vs okur
  */
  const router = useRouter();

  /*
    Sipariş özetini oluşturmak için
    sepet bilgilerine ulaşıyoruz.
  */
  const {
    cartItems,
    totalQuantity,
    totalPrice,
  } = useCart();

  /*
    Kullanıcı formdaki butona bastiginda calisan fonks
  */
  function handleSubmit(
    event: FormEvent<HTMLFormElement>
  ) {
    /*
      Formun tarayıcı tarafından normal şekilde
      gönderilmesini ve sayfanın yenilenmesini engeller.
    */
    event.preventDefault();

    /*
      Sepet boşsa ödeme işlemine devam edilmez.
    */
    if (cartItems.length === 0) {
      return;
    }

    /*
      Tarayıcının required, pattern ve minLength gibi
      kontrolleri başarılı olduğunda kullanıcıyı
      3D Secure ekranına yönlendiriyoruz.
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

      <section className={styles.checkoutContainer}>
        <div className={styles.pageHeader}>
          <span className={styles.label}>
            GÜVENLİ ÖDEME
          </span>

          <h1>Ödeme Bilgileri</h1>

          <p>
            Teslimat ve kart bilgilerini girerek
            siparişini tamamlamaya devam edebilirsin.
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

            <h2>Ödeme için sepetinde ürün bulunmalı</h2>

            <p>
              Ürünleri inceleyerek alışveriş sepetine
              en az bir ürün eklemelisin.
            </p>

            <Link href="/#products">
              Ürünleri Keşfet
            </Link>
          </div>
        ) : (
          /*
            HTML form kontrolleri yapilir. Boylece kullanicidan gelen
            input ozelliklerine bakarak temek kontrolleri yapar.
            Bu alan bos birakilamaz,Kullanıcının beş karakterden fazla girmesini engellenir,
            Kullanıcı alanı doldurmadan butona basarsa tarayıcı formu göndermez ve 
            uyarı gösterir gibi.
          */
          <form
            className={styles.checkoutGrid}
            onSubmit={handleSubmit}
          >
            <div className={styles.formSections}>
              {/*
                Teslimat bilgilerinin bulunduğu bölüm.
              */}
              <section className={styles.formCard}>
                <div className={styles.sectionHeader}>
                  <div className={styles.sectionIcon}>
                    <MapPin size={23} />
                  </div>

                  <div>
                    <h2>Teslimat Bilgileri</h2>

                    <p>
                      Siparişin teslim edileceği adresi gir.
                    </p>
                  </div>
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="fullName">
                    Ad Soyad
                  </label>

                  <input
                    id="fullName"
                    name="fullName"
                    type="text"
                    placeholder="Adını ve soyadını gir"
                    autoComplete="name"
                    required
                  />
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="phone">
                    Telefon numarası
                  </label>

                  <input
                    id="phone"
                    name="phone"
                    type="tel"
                    placeholder="05XX XXX XX XX"
                    autoComplete="tel"
                    required
                  />
                </div>

                {/*
                  İl ve ilçe alanlarını yan yana göstermek
                  için formRow kullanıyoruz.
                */}
                <div className={styles.formRow}>
                  <div className={styles.formGroup}>
                    <label htmlFor="city">
                      İl
                    </label>

                    <input
                      id="city"
                      name="city"
                      type="text"
                      placeholder="İstanbul"
                      autoComplete="address-level1"
                      required
                    />
                  </div>

                  <div className={styles.formGroup}>
                    <label htmlFor="district">
                      İlçe
                    </label>

                    <input
                      id="district"
                      name="district"
                      type="text"
                      placeholder="Kadıköy"
                      autoComplete="address-level2"
                      required
                    />
                  </div>
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="address">
                    Açık adres
                  </label>

                  <textarea
                    id="address"
                    name="address"
                    placeholder="Mahalle, cadde, sokak, bina ve daire bilgilerini gir"
                    autoComplete="street-address"
                    rows={4}
                    required
                  />
                </div>
              </section>

              {/*
                Kart bilgilerinin bulunduğu bölüm.
                Bu bilgiler localStorage içerisine kaydedilmez.
              */}
              <section className={styles.formCard}>
                <div className={styles.sectionHeader}>
                  <div className={styles.sectionIcon}>
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
                    name="cardHolder"
                    type="text"
                    placeholder="AD SOYAD"
                    autoComplete="cc-name"
                    required
                  />
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="cardNumber">
                    Kart numarası
                  </label>

                  <div className={styles.secureInput}>
                    <CreditCard
                      size={19}
                      aria-hidden="true"
                    />

                    <input
                      id="cardNumber"
                      name="cardNumber"
                      type="text"

                      /*
                        inputMode="numeric", mobil cihazlarda
                        sayısal klavyenin açılmasını sağlar.
                      */
                      inputMode="numeric"

                      placeholder="0000 0000 0000 0000"
                      autoComplete="cc-number"

                      /*
                        Boşluklarla birlikte en fazla
                        19 karakter girilebilir.
                      */
                      maxLength={19}

                      /*
                        Yalnızca rakam ve boşluklardan oluşan
                        15-19 karakterlik değeri kabul eder.
                      */
                      pattern="[0-9 ]{15,19}"
                      required
                    />
                  </div>
                </div>

                <div className={styles.formRow}>
                  <div className={styles.formGroup}>
                    <label htmlFor="expiryDate">
                      Son kullanma tarihi
                    </label>

                    <input
                      id="expiryDate"
                      name="expiryDate"
                      type="text"
                      inputMode="numeric"
                      placeholder="AA/YY"
                      autoComplete="cc-exp"
                      maxLength={5}

                      /*
                        01/26 ile 12/99 arasındaki
                        AA/YY biçimini kontrol eder.
                      */
                      pattern="(0[1-9]|1[0-2])\/[0-9]{2}"
                      required
                    />
                  </div>

                  <div className={styles.formGroup}>
                    <label htmlFor="cvv">
                      CVV
                    </label>

                    <div className={styles.secureInput}>
                      <LockKeyhole
                        size={18}
                        aria-hidden="true"
                      />

                      <input
                        id="cvv"
                        name="cvv"
                        type="password"
                        inputMode="numeric"
                        placeholder="123"
                        autoComplete="cc-csc"
                        maxLength={4}

                        /*
                          CVV değerinin yalnızca
                          3 veya 4 rakam olmasını ister.
                        */
                        pattern="[0-9]{3,4}"
                        required
                      />
                    </div>
                  </div>
                </div>

                <label className={styles.secureAgreement}>
                  <input
                    name="useThreeDSecure"
                    type="checkbox"
                    required
                  />

                  <span>
                    Ödemenin 3D Secure doğrulamasıyla
                    tamamlanmasını kabul ediyorum.
                  </span>
                </label>

                <div className={styles.securityInformation}>
                  <ShieldCheck size={21} />

                  <p>
                    Kart bilgilerin TechCart tarafından
                    saklanmaz. Ödeme işlemi güvenli ödeme
                    altyapısı üzerinden yürütülecektir.
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
                      {(
                        item.product.price *
                        item.quantity
                      ).toLocaleString("tr-TR")}{" "}
                      ₺
                    </strong>
                  </div>
                ))}
              </div>

              <div className={styles.summaryRow}>
                <span>Ürün adedi</span>
                <strong>{totalQuantity}</strong>
              </div>

              <div className={styles.summaryRow}>
                <span>Ara toplam</span>

                <strong>
                  {totalPrice.toLocaleString("tr-TR")} ₺
                </strong>
              </div>

              <div className={styles.summaryRow}>
                <span>Kargo</span>
                <strong>Ücretsiz</strong>
              </div>

              <div className={styles.totalRow}>
                <span>Toplam</span>

                <strong>
                  {totalPrice.toLocaleString("tr-TR")} ₺
                </strong>
              </div>

              {/*
                Form başarılı şekilde doğrulanırsa
                handleSubmit fonksiyonu çalışır.
              */}
              <button
                className={styles.paymentButton}
                type="submit"
              >
                <LockKeyhole size={19} />
                3D Secure ile Ödemeye Geç
              </button>

            </aside>
          </form>
        )}
      </section>
    </main>
  );
}