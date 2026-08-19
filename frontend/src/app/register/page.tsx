import {
  ArrowLeft,
  LockKeyhole,
  Mail,
  ShieldCheck,
  UserRound,
} from "lucide-react";

import Link from "next/link";
import styles from "./page.module.css";

export default function RegisterPage() {
  return (
    
    <main className={styles.registerPage}>
      <div className={styles.registerContainer}>
        {/*
        Sol taraftaki tanıtım alanı*/}
        <section className={styles.introduction}>
         
          <Link className={styles.logo} href="/">
            TECHCART
          </Link>

          <div className={styles.introductionContent}>
            <span className={styles.label}>
              TECHCART ÜYELİĞİ
            </span>

            <h1>
              Alışveriş deneyimini kişiselleştir.
            </h1>

            <p>
              Hesabını oluştur, ürünleri sepetine
              ekle, siparişlerini takip et ve satın aldığın
              ürünleri değerlendir.
            </p>

            <ul className={styles.benefitList}>
              <li>
                <ShieldCheck size={20} />
                Güvenli hesap ve sipariş yönetimi
              </li>

              <li>
                <ShieldCheck size={20} />
                Sepet ve sipariş geçmişine erişim
              </li>

              <li>
                <ShieldCheck size={20} />
                Satın alınan ürünleri puanlama
              </li>
            </ul>
          </div>

          <Link className={styles.backLink} href="/">
            <ArrowLeft size={18} />
            Ana sayfaya dön
          </Link>
        </section>

        <section className={styles.formSection}>
          <div className={styles.formHeader}>
            <h2>Hesap Oluştur</h2>

            <p>
              TechCart’a kayıt olmak için bilgilerini gir.
            </p>
          </div>

          <form className={styles.registerForm}>
          
            <div className={styles.formRow}>
              <div className={styles.formGroup}>
                <label htmlFor="firstName">
                  Ad
                </label>

                <div className={styles.inputWrapper}>
                  <UserRound
                    className={styles.inputIcon}
                    size={20}
                    aria-hidden="true"
                  />

                  <input
                    id="firstName"
                    name="firstName"
                    type="text"
                    placeholder="Adını gir"
                    autoComplete="given-name"
                    required
                  />
                </div>
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="lastName">
                  Soyad
                </label>

                <div className={styles.inputWrapper}>
                  <UserRound
                    className={styles.inputIcon}
                    size={20}
                    aria-hidden="true"
                  />

                  <input
                    id="lastName"
                    name="lastName"
                    type="text"
                    placeholder="Soyadını gir"
                    autoComplete="family-name"
                    required
                  />
                </div>
              </div>
            </div>

            <div className={styles.formGroup}>
              <label htmlFor="email">
                E-posta adresi
              </label>

              <div className={styles.inputWrapper}>
                <Mail
                  className={styles.inputIcon}
                  size={20}
                  aria-hidden="true"
                />

                <input
                  id="email"
                  name="email"
                  type="email"
                  placeholder="ornek@techcart.com"
                  autoComplete="email"
                  required
                />
              </div>
            </div>

            <div className={styles.formRow}>
              <div className={styles.formGroup}>
                <label htmlFor="password">
                  Parola
                </label>

                <div className={styles.inputWrapper}>
                  <LockKeyhole
                    className={styles.inputIcon}
                    size={20}
                    aria-hidden="true"
                  />

                  <input
                    id="password"
                    name="password"
                    type="password"
                    placeholder="Parolanı gir"
                    autoComplete="new-password"
                    minLength={8}
                    required
                  />
                </div>
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="confirmPassword">
                  Parola tekrarı
                </label>

                <div className={styles.inputWrapper}>
                  <LockKeyhole
                    className={styles.inputIcon}
                    size={20}
                    aria-hidden="true"
                  />

                  <input
                    id="confirmPassword"
                    name="confirmPassword"
                    type="password"
                    placeholder="Parolanı tekrar gir"
                    autoComplete="new-password"
                    minLength={8}
                    required
                  />
                </div>
              </div>
            </div>

            <p className={styles.passwordInformation}>
              Parolan en az 8 karakterden oluşmalıdır.
            </p> 

            {/*
              type="checkbox" kullanıcıya işaretlenebilir
              bir onay kutusu sunar.  required kullanıldığı için
              kullanıcı koşulları kabul etmeden formu gönderemez.
            */}
            <label className={styles.termsArea}>
              <input
                name="acceptTerms"
                type="checkbox"
                required
              />

              <span>
                <Link href="/kullanim-kosullari">
                  Kullanım Koşulları
                </Link>

                {" ve "}

                <Link href="/gizlilik-politikasi">
                  Gizlilik Politikası
                </Link>

                ’nı okudum ve kabul ediyorum.
              </span>
            </label>

            <button
              className={styles.registerButton}
              type="submit"
            >
              Hesap Oluştur
            </button>
          </form>

          <div className={styles.loginArea}>
            <span>Zaten hesabın var mı?</span>

            <Link href="/login">
              Giriş Yap
            </Link>
          </div>
        </section>
      </div>
    </main>
  );
}