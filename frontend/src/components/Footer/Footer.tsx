import Link from "next/link";
import styles from "./Footer.module.css";

export default function Footer() {

    // yili otomatik olusturuyoruz.
    const currentYear = new Date().getFullYear();

return (
     /*
     footer etiketi, sayfanin en altindaki genel bilgi, 
     iletisim vs icin kullanilir
    */
    <footer className={styles.footer}>
      <div className={styles.footerContainer}>
       
        <div className={styles.footerGrid}>
          <div className={styles.brandColumn}>
            <Link href="/" className={styles.logo}>
              TechCart
            </Link>

            <p className={styles.brandDescription}>
              Teknoloji ürünlerini keşfetmenin, karşılaştırmanın
              ve güvenle seçmenin kolay yolu.
            </p>
          </div>

          {/*nav etiketi, kullaniciyi farkli sayfalara yonlendiren 
        baglantilari fruplamak icin */}
          <nav
            className={styles.footerColumn}
            aria-label="Kurumsal bağlantılar"
          >
            <h2>Kurumsal</h2>

            <Link href="/about">Hakkımızda</Link>
            <Link href="/career">Kariyer</Link>
            <Link href="/press">Basın</Link>
            <Link href="/blog">Blog</Link>
          </nav>

          <nav
            className={styles.footerColumn}
            aria-label="Müşteri hizmetleri bağlantıları"
          >
            <h2>Müşteri Hizmetleri</h2>

            <Link href="/orders">Sipariş Takibi</Link>
            <Link href="/returns">İade ve Değişim</Link>
            <Link href="/warranty">Garanti</Link>
            <Link href="/faq">Sık Sorulan Sorular</Link>
          </nav>

         {/*adres etiketi, telefon, e-posta ve adres gibi
           iletisim bilgilerini gruplamak icin */}
          <address className={styles.contactColumn}>
            <h2>İletişim</h2>
           
            <a href="tel:+908501234567">
              0850 123 45 67
            </a>
    
            <a href="mailto:destek@techcart.com.tr">
              destek@techcart.com.tr
            </a>

            <p>İstanbul, Türkiye</p>
          </address>
        </div>
       
        <div className={styles.divider} />

        <div className={styles.footerBottom}>
          <p>
            © {currentYear} TechCart. Tüm hakları saklıdır.
          </p>

          <nav
            className={styles.legalLinks}
            aria-label="Yasal bağlantılar"
          >
            <Link href="/privacy">
              Gizlilik Politikası
            </Link>

            <Link href="/terms">
              Kullanım Koşulları
            </Link>
          </nav>
        </div>
      </div>
    </footer>
  );
}