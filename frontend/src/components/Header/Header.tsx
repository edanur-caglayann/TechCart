import { ShoppingCart, User, UserPlus } from "lucide-react";
import Link from "next/link";
import SearchBar from "../SearchBar/SearchBar";
import styles from "./Header.module.css";

export default function Header() {
  return (
    // header bileseni sayfanin ust bolumunu olusturur ve icinde logo, arama kutusu ve menu linklerini barindirir.
    <header className={styles.header}>
        <Link href="/" className={styles.logo}> 
            TechCart
        </Link>
        <SearchBar /> 
        
       <nav className={styles.menu}>
        <Link href="/login" className={styles.loginLink}>
          <User size={20} strokeWidth={1.8} />
          <span>Giriş Yap</span>
        </Link>

        <Link href="/register" className={styles.registerLink}>
          <UserPlus size={20} strokeWidth={1.8} />
          <span>Kayıt Ol</span>
        </Link>

        <Link href="/cart" className={styles.cartLink}>
          <ShoppingCart size={20} strokeWidth={1.8} />
          <span>Sepetim</span>
        </Link>
      </nav>
    </header>
  );
}
