import styles from './Filters.module.css';

export default function Filters() {
    return (
        /* Filtre panelinin tamamini kapsar */
        <aside className={styles.filterPanel}>
            <div className={styles.filterHeader}>
                <h2>Filtreler</h2>

        <button className={styles.clearButton}
        type="button">
            Temizle
        </button>
        </div>
        
        <form className={styles.filterForm}>
            <div className={styles.filterGroup}>
                <h3>Kategori</h3>

              <label className={styles.checkboxLabel}>
            <input type="checkbox" name="category" value="telefon" />
            <span>Telefon</span>
          </label>

          <label className={styles.checkboxLabel}>
            <input type="checkbox" name="category" value="bilgisayar" />
            <span>Bilgisayar</span>
          </label>

          <label className={styles.checkboxLabel}>
            <input type="checkbox" name="category" value="tablet" />
            <span>Tablet</span>
          </label>

          <label className={styles.checkboxLabel}>
            <input type="checkbox" name="category" value="aksesuar" />
            <span>Aksesuar</span>
          </label>

        <label className={styles.checkboxLabel}>
          <input type="checkbox" name="category" value="televizyon" />
          <span>Televizyon</span>
        </label>

        <label className={styles.checkboxLabel}>
          <input type="checkbox" name="category" value="akilli-saat" />
          <span>Akıllı Saat</span>
        </label>


        <label className={styles.checkboxLabel}>
          <input type="checkbox" name="category" value="ses-sistemi" />
          <span>Ses Sistemleri</span>
        </label>

        <label className={styles.checkboxLabel}>
          <input type="checkbox" name="category" value="oyun-konsolu" />
          <span>Oyun Konsolu</span>
        </label>


        <label className={styles.checkboxLabel}>
          <input type="checkbox" name="category" value="pikap" />
          <span>Pikap</span>
        </label>
          
        </div>     
        
        <div className={styles.filterGroup}>
          <label className={styles.groupTitle} htmlFor="brand">
            Marka
          </label>

         <select className={styles.select} id="brand" name="brand">
            <option value="">Tüm markalar</option>

            <option value="apple">Apple</option>
            <option value="samsung">Samsung</option>
            <option value="xiaomi">Xiaomi</option>
            <option value="lenovo">Lenovo</option>
            <option value="asus">Asus</option>
            <option value="logitech">Logitech</option>

            <option value="sony">Sony</option>
            <option value="lg">LG</option>
            <option value="philips">Philips</option>
            <option value="tcl">TCL</option>

            <option value="audio-technica">Audio-Technica</option>
            <option value="pioneer">Pioneer</option>
            <option value="jbl">JBL</option>
            <option value="marshall">Marshall</option>

            <option value="microsoft">Microsoft</option>
            <option value="nintendo">Nintendo</option>
        </select>
        </div>

        <div className={styles.filterGroup}>
          <h3>Fiyat Aralığı</h3>

          <div className={styles.priceInputs}>
            <div>
              <label htmlFor="minPrice">En az</label>

              <input
                id="minPrice"
                name="minPrice"
                type="number"
                min="0"
                placeholder="0 ₺"
              />
            </div>

            <div>
              <label htmlFor="maxPrice">En fazla</label>

              <input
                id="maxPrice"
                name="maxPrice"
                type="number"
                min="0"
                placeholder="100.000 ₺"
              />
            </div>
          </div>
        </div>

        <div className={styles.filterGroup}>
          <h3>Renk</h3>

          <div className={styles.colorList}>
            <label className={styles.colorOption}>
              <input type="checkbox" name="color" value="siyah" />
              <span
                className={`${styles.colorCircle} ${styles.black}`}
              />
              <span>Siyah</span>
            </label>

            <label className={styles.colorOption}>
              <input type="checkbox" name="color" value="beyaz" />
              <span
                className={`${styles.colorCircle} ${styles.white}`}
              />
              <span>Beyaz</span>
            </label>

            <label className={styles.colorOption}>
              <input type="checkbox" name="color" value="gri" />
              <span
                className={`${styles.colorCircle} ${styles.gray}`}
              />
              <span>Gri</span>
            </label>

            <label className={styles.colorOption}>
              <input type="checkbox" name="color" value="mavi" />
              <span
                className={`${styles.colorCircle} ${styles.blue}`}
              />
              <span>Mavi</span>
            </label>
          </div>
        </div>

        <div className={styles.filterGroup}>
          <h3>Stok Durumu</h3>

          <label className={styles.checkboxLabel}>
            <input type="checkbox" name="inStock" />
            <span>Yalnızca stokta olanlar</span>
          </label>
        </div>

        <div className={styles.filterGroup}>
          <label className={styles.groupTitle} htmlFor="rating">
            Minimum Puan
          </label>

          <select className={styles.select} id="rating" name="rating">
            <option value="">Tüm puanlar</option>
            <option value="4">4 yıldız ve üzeri</option>
            <option value="3">3 yıldız ve üzeri</option>
            <option value="2">2 yıldız ve üzeri</option>
          </select>
        </div>

        <button className={styles.applyButton} type="button">
          Filtreleri Uygula
        </button>
      </form> 
    </aside>
  );
}