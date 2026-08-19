import {
  Headphones,
  Laptop,
  Smartphone,
  Tablet,
} from "lucide-react";

import Link from "next/link";
import styles from "./CategoryMenu.module.css";

export default function CategoryMenu() {
  return (
    /*section kullanarak kategorilerin ana sayfa icinde bir bolum olarak tutulmasini sagladik
    styles.categoryGrid -> css dosyasinda tanimlanan grid yapisini kullanarak 
    kategorileri duzenli bir sekilde gosterir. Dort kategori  kartini bir arada tutar
    <Link 
        href="/?category=telefon#products" 
        kategori kartinin tamamini tiklanabilr yapar

    <Smartphone
          className={styles.backgroundIcon}
          kartin arka tarafinda ekledigimiz buyuk silik ikon
    */ 
    <section className={styles.categorySection}>
      <div className={styles.sectionHeader}>
        <div> 
          <span className={styles.sectionLabel}>Kategoriler</span>
          <h2 className={styles.sectionTitle}>Aradığın teknolojiye kolayca ulaş</h2>
          </div>

          <p className={styles.sectionDescription}>
             İhtiyacına uygun ürünleri kategoriye göre incele.
        </p>
      </div>
   
      <div className={styles.categoryGrid}>
        <Link 
        href="/?category=telefon#products"
        className={`${styles.categoryCard} ${styles.phoneCard}`} 
        >
         
          <Smartphone
          className={styles.backgroundIcon}
          strokeWidth={1.1}
          />

          <div className={styles.cardContent}>
            <div className={styles.iconBox}>
              <Smartphone size = {28} strokeWidth={1.7} />
              </div>

              <h3>Telefon</h3>
              <p>56 Ürün</p>
          </div>
        </Link>
   <Link
          href="/?category=bilgisayar#products"
          className={`${styles.categoryCard} ${styles.computerCard}`}
        >
          <Laptop
            className={styles.backgroundIcon}
            strokeWidth={1.1}
          />

          <div className={styles.cardContent}>
            <div className={styles.iconBox}>
              <Laptop size={29} strokeWidth={1.7} />
            </div>

            <h3>Bilgisayar</h3>
            <p>74 ürün</p>
          </div>
        </Link>

        <Link
          href="/?category=tablet#products"
          className={`${styles.categoryCard} ${styles.tabletCard}`}
        >
          <Tablet
            className={styles.backgroundIcon}
            strokeWidth={1.1}
          />

          <div className={styles.cardContent}>
            <div className={styles.iconBox}>
              <Tablet size={28} strokeWidth={1.7} />
            </div>

            <h3>Tablet</h3>
            <p>32 ürün</p>
          </div>
        </Link>

        <Link
          href="/?category=aksesuar#products"
          className={`${styles.categoryCard} ${styles.accessoryCard}`}
        >
          <Headphones
            className={styles.backgroundIcon}
            strokeWidth={1.1}
          />

          <div className={styles.cardContent}>
            <div className={styles.iconBox}>
              <Headphones size={29} strokeWidth={1.7} />
            </div>

            <h3>Aksesuar</h3>
            <p>25 ürün</p>
          </div>
        </Link>
      </div>
    </section>
  );
}