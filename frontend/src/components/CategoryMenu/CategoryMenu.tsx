import styles from "./CategoryMenu.module.css";

export default function CategoryMenu() {
  return (
    <nav className={styles.categoryMenu}>
      <button className={styles.activeButton} type="button">
        Tüm Ürünler
      </button>

      <button className={styles.categoryButton} type="button">
        Bilgisayar
      </button>

      <button className={styles.categoryButton} type="button">
        Telefon
      </button>

      <button className={styles.categoryButton} type="button">
        Tablet
      </button>

      <button className={styles.categoryButton} type="button">
        Kulaklık
      </button>

      <button className={styles.categoryButton} type="button">
        Aksesuar
      </button>
    </nav>
  );
}