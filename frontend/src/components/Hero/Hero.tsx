import { Headphones, Laptop, Smartphone } from "lucide-react";
import styles from "./Hero.module.css";

export default function Hero() {
    return (
        // Hero alaninin tamamini kapsar
        <section className={styles.hero}>
            <div className={styles.content}>
                <span className={styles.label}>
                   Teknolojinin yeni adresi
                   </span>
                <h1 className={styles.title}>
                      Teknolojiyi Seçmenin 
          <br /> 
          Kolay Yolu
        </h1>
        <p className={styles.description}>
          Ürünleri keşfet, özelliklerini karşılaştır, ihtiyacına uygun olanı güvenle seç.
        </p>

        <div className={styles.actions}>
          <a className={styles.discoverButton} href="#products">
            Ürünleri Keşfet
            <span aria-hidden="true">→</span>
          </a>

          <div className={styles.customerInfo}>
            <strong>50+</strong>
            <span>teknoloji ürünü</span>
          </div>
        </div>
      </div>

      <div className={styles.visual}>
        <div className={styles.visualContent}>
          <Laptop className={styles.laptopIcon} strokeWidth={1.3} />

          <div className={styles.smallIcons}>
            <div className={styles.iconBox}>
              <Smartphone size={38} strokeWidth={1.5} />
            </div>

            <div className={styles.iconBox}>
              <Headphones size={38} strokeWidth={1.5} />
            </div>
          </div>
        </div>

        <div className={styles.visualNote}>
          <span className={styles.noteIcon}>✓</span>

          <div>
            <strong>Güvenli Alışveriş</strong>
            <p>Teknoloji ürünleri tek adreste</p>
          </div>
        </div>
      </div>
    </section>
  );
  }