import { Search } from "lucide-react"; // buyutec ikonunu dosyaya getirir

// Arama kutusunun gorunumunu belirleyecek olan CSS dosyasini import ettik
import styles from './SearchBar.module.css'; 

export default function SearchBar() {
    return (
        // SearchBar adinda bir react bileseni olusturduk.
        <div className={styles.searchBar}>
            <input
            className={styles.input}
                type="text" // urun adini yazabilecegi metin kutusu
                placeholder="Ürün, marka veya kategori ara..." 
                // kullanici henuz bir sey yazmamisken kutunun icinde gorunecek olan aciklama
            />
            
            <button className={styles.button} 
            type = "button"
            aria-label="Ürün ara"
            >
    
            <Search size={21} strokeWidth={2} />
            </button>
        </div>
    );
}