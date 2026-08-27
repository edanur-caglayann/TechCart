'use client';

// filtre secimleri direkt URL query parametrelerine yazilmasi icin import ettik. ?category=telefon&brand=apple gibi 
import { useRouter, useSearchParams } from 'next/navigation';
import { FormEvent } from 'react';
import styles from './Filters.module.css';

export default function Filters() {
    // Next.js yönlendirme ve URL parametrelerini okuma araçları (URL State yönetimi için)
    const router = useRouter();
    const searchParams = useSearchParams();

    // URL'den tekli parametreleri okumaya yardımcı fonksiyon
    const getParam = (key: string) => searchParams.get(key) || '';
    // URL'den aynı isimli çoklu parametreleri (örn: birden fazla kategori/renk) okumaya yardımcı fonksiyon
    const getAllParams = (key: string) => searchParams.getAll(key);

    // Toplu Uygulama (Batch Filtering) -> Kullanıcı her seçim yaptığında değil,
    // Filtreleri Uygula butonuna basıldığı anda tüm form verileri URL'e aktarılır.
    const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const formData = new FormData(e.currentTarget);
        const params = new URLSearchParams();

        // Form içerisindeki tüm seçili alanları tarayıp URL parametrelerine ekliyoruz
        formData.forEach((value, key) => {
            if (value) {
                params.append(key, value.toString());
            }
        });

        router.push(`?${params.toString()}`);
    };

    // Filtrelerin Sıfırlanması: Temizle butonuna basıldığında URL parametreleri temizlenir.
    const handleReset = () => {
        router.push(window.location.pathname);
    };

    return (
        /* Filtre panelinin tamamini kapsar */
        <aside className={styles.filterPanel}>
            <div className={styles.filterHeader}>
                <h2>Filtreler</h2>

                {/* Filtreleri sıfırlayan temizle butonu */}
                <button 
                    className={styles.clearButton}
                    type="button"
                    onClick={handleReset}
                >
                    Temizle
                </button>
            </div>
        
            {/* onSubmit ile Batch Filtering (Toplu Uygulama) mantığı ile calisir
            Kullanici filtreleri isaretlerken sayfa aninda yenilenmez. Bunun yerine tum secimler <form>
            etiketinin icinde tutulur. Filtreleri uygula denildikten sonra(type=submit)butonuna basinca 
            onSubmit tetiklenir ve tum veriler tek seferde toplanip URL'e gonderilir
            */}
            <form className={styles.filterForm} onSubmit={handleSubmit}>
                <div className={styles.filterGroup}>
                    <h3>Kategori</h3>

                    {/* defaultChecked ile sayfa yenilense bile URL'deki seçimlerin seçili gelmesi sağlanır */}
                    <label className={styles.checkboxLabel}>
                        <input type="checkbox" name="category" value="telefon" defaultChecked={getAllParams('category').includes('telefon')} />
                        <span>Telefon</span>
                    </label>

                    <label className={styles.checkboxLabel}>
                        <input type="checkbox" name="category" value="bilgisayar" defaultChecked={getAllParams('category').includes('bilgisayar')} />
                        <span>Bilgisayar</span>
                    </label>

                    <label className={styles.checkboxLabel}>
                        <input type="checkbox" name="category" value="tablet" defaultChecked={getAllParams('category').includes('tablet')} />
                        <span>Tablet</span>
                    </label>

                    <label className={styles.checkboxLabel}>
                        <input type="checkbox" name="category" value="aksesuar" defaultChecked={getAllParams('category').includes('aksesuar')} />
                        <span>Aksesuar</span>
                    </label>

                    <label className={styles.checkboxLabel}>
                        <input type="checkbox" name="category" value="televizyon" defaultChecked={getAllParams('category').includes('televizyon')} />
                        <span>Televizyon</span>
                    </label>

                    <label className={styles.checkboxLabel}>
                        <input type="checkbox" name="category" value="akilli-saat" defaultChecked={getAllParams('category').includes('akilli-saat')} />
                        <span>Akıllı Saat</span>
                    </label>

                    <label className={styles.checkboxLabel}>
                        <input type="checkbox" name="category" value="ses-sistemi" defaultChecked={getAllParams('category').includes('ses-sistemi')} />
                        <span>Ses Sistemleri</span>
                    </label>

                    <label className={styles.checkboxLabel}>
                        <input type="checkbox" name="category" value="oyun-konsolu" defaultChecked={getAllParams('category').includes('oyun-konsolu')} />
                        <span>Oyun Konsolu</span>
                    </label>

                    <label className={styles.checkboxLabel}>
                        <input type="checkbox" name="category" value="pikap" defaultChecked={getAllParams('category').includes('pikap')} />
                        <span>Pikap</span>
                    </label>
                </div>     
        
                <div className={styles.filterGroup}>
                    <label className={styles.groupTitle} htmlFor="brand">
                        Marka
                    </label>

                    {/* defaultValue ile URL'den gelen marka seçimi select kutusunda korunur */}
                    <select className={styles.select} id="brand" name="brand" defaultValue={getParam('brand')}>
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
                                defaultValue={getParam('minPrice')}
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
                                defaultValue={getParam('maxPrice')}
                            />
                        </div>
                    </div>
                </div>

                <div className={styles.filterGroup}>
                    <h3>Renk</h3>

                    <div className={styles.colorList}>
                        <label className={styles.colorOption}>
                            <input type="checkbox" name="color" value="siyah" defaultChecked={getAllParams('color').includes('siyah')} />
                            <span className={`${styles.colorCircle} ${styles.black}`} />
                            <span>Siyah</span>
                        </label>

                        <label className={styles.colorOption}>
                            <input type="checkbox" name="color" value="beyaz" defaultChecked={getAllParams('color').includes('beyaz')} />
                            <span className={`${styles.colorCircle} ${styles.white}`} />
                            <span>Beyaz</span>
                        </label>

                        <label className={styles.colorOption}>
                            <input type="checkbox" name="color" value="gri" defaultChecked={getAllParams('color').includes('gri')} />
                            <span className={`${styles.colorCircle} ${styles.gray}`} />
                            <span>Gri</span>
                        </label>

                        <label className={styles.colorOption}>
                            <input type="checkbox" name="color" value="mavi" defaultChecked={getAllParams('color').includes('mavi')} />
                            <span className={`${styles.colorCircle} ${styles.blue}`} />
                            <span>Mavi</span>
                        </label>
                    </div>
                </div>

                <div className={styles.filterGroup}>
                    <h3>Stok Durumu</h3>

                    <label className={styles.checkboxLabel}>
                        <input type="checkbox" name="inStock" value="true" defaultChecked={searchParams.has('inStock')} />
                        <span>Yalnızca stokta olanlar</span>
                    </label>
                </div>

                {/* type="submit" ile form verileri yakalanıp URL'e gönderilir */}
                <button className={styles.applyButton} type="submit">
                    Filtreleri Uygula
                </button>
            </form> 
        </aside>
    );
}