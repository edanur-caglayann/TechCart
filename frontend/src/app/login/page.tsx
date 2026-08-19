import {
    ArrowLeft,
    LockKeyhole,
    Mail,
} from "lucide-react";

import Link from "next/link";
import styles from "./page.module.css";

export default function LoginPage() {
    return (
        /* main, giris sayfasinin kapsayicisidir.
        Sayfadaki butun alanlari ortalamak icin kullanilir.
        */ 
        <main className={styles.loginPage}>
            <div className={styles.loginContainer}>
                <section className={styles.introduction}>
                    <Link className={styles.logo} href="/">
                    TECHCART
                    </Link>

                    <div className={styles.introductionContent}>
                        <span className={styles.label}>
                             GÜVENLİ ALIŞVERİŞ
                        </span>

                        <h1>
                            Alışverişine kaldığın yerden devam et.
                        </h1>

                        <p>
                            Hesabına giriş yaparak sepetini yönetebilir,
                            siparişlerini takip edebilir ve ürünleri
                            değerlendirebilirsin.
                        </p>
                    </div>
                    
                    {/* Link bileşeni kullanıcıyı ana sayfaya yönlendirir.*/}
                    <Link className={styles.backLink} href="/">
                     <ArrowLeft size={18} /> {/* baglantinin yanidnaki geri ok ikonu */}
                        Ana sayfaya dön
                     </Link>
                </section>

                 <section className={styles.formSection}>
             <div className={styles.formHeader}>
                 <h2>Giriş Yap</h2>

             <p>
              TechCart hesabına erişmek için bilgilerini gir.
             </p>
            </div>
             <form className={styles.loginForm}>
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
                  autoComplete="email" // kayitli mail adreslerini onerir
                  required // alani bos birakamaz
                />
              </div>
            </div>

            {/* parola basligini, parolami unuttum baglantisini ve 
            parola kutusunu tek grup icinde tutar
            */}
            <div className={styles.formGroup}>
              <div className={styles.passwordHeader}>
                <label htmlFor="password">
                  Parola
                </label>

                <Link href="/forgot-password">
                  Parolamı unuttum
                </Link>
              </div>

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
                  autoComplete="current-password" // tarayici parolayi kaydettiyse onermesin
                  required
                />
              </div>
            </div>

            <button
              className={styles.loginButton}
              type="submit"
            >
              Giriş Yap
            </button>
          </form>

          <div className={styles.registerArea}>
            <span>Henüz hesabın yok mu?</span>

            <Link href="/register">
              Kayıt Ol
            </Link>
          </div>
        </section>
      </div>
    </main>
  );
}