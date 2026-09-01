/*
  AuthContext, menü açma-kapama durumu ve tarayıcı
  olayları kullanıldığı için Header Client Component'tir.
*/
"use client";

import {
  ChevronDown,
  LogOut,
  MapPin,
  Package,
  User,
  UserCircle,
  UserPlus,
} from "lucide-react";

import Link from "next/link";
import {
  useEffect,
  useRef,
  useState,
} from "react";
import {
  usePathname,
  useRouter,
} from "next/navigation";

import { useCart } from "../../context/CartContext";

import { useAuth } from "../../context/AuthContext";
import SearchBar from "../SearchBar/SearchBar";
import styles from "./Header.module.css";

export default function Header() {

  //Kullanıcının oturum durumuna, bilgilerine ve çıkış fonksiyonuna AuthContext üzerinden ulaşırız.
  const {
    user, // giris yapan kullanici bilgileri
    isAuthenticated, // giris yapmis mi
    isAuthLoading, //localstorage kontrlu
    logout, // kullanici oturum bilgisini temizleyen fonks
  } = useAuth();

  // Sepetteki toplam ürün adedini alırız.
  const { totalQuantity } = useCart();
  //Hesabım menüsünün açık veya kapalı olduğunu tutar.
  const [isAccountMenuOpen, setIsAccountMenuOpen] =
    useState(false);

  /*
    accountMenuRef, Hesabım butonuyla açılır menünün
    bulunduğu HTML alanına erişmemizi sağlar.
  */
  const accountMenuRef =
    useRef<HTMLDivElement | null>(null);

  const router = useRouter();
  const pathname = usePathname();

  //Kullanıcı menü dışında bir yere tıkladığında Hesabım menüsü kapailir
  useEffect(() => {
    function closeMenuOnOutsideClick(
      event: MouseEvent
    ) {
      const clickedElement = event.target as Node;

      // tiklanan yer hesabim menusunun disinda mi
      if (
        accountMenuRef.current &&
        !accountMenuRef.current.contains(
          clickedElement
        )
      ) {
        setIsAccountMenuOpen(false);
      }
    }

    document.addEventListener(
      "mousedown",
      closeMenuOnOutsideClick
    );

    /*
      Header ekrandan kaldırılırsa eklediğimiz
      document olayını temizleriz.
    */
    return () => {
      document.removeEventListener(
        "mousedown",
        closeMenuOnOutsideClick
      );
    };
  }, []);

  /*
    Kullanıcı başka bir URL'ye geçtiğinde açık kalan
    Hesabım menüsünü otomatik olarak kapatırız.
  */
  useEffect(() => {
    setIsAccountMenuOpen(false);
  }, [pathname]);

  /*
    Çıkış Yap seçildiğinde AuthContext içerisindeki
    kullanıcı bilgisi temizlenir ve ana sayfaya gidilir.
  */
  function handleLogout() {
    logout();
    setIsAccountMenuOpen(false);
    router.push("/");
  }

  return (
    /*
      Header sayfanın üst bölümünü oluşturur ve
      logo, arama kutusu ile menüleri barındırır.
    */
    <header className={styles.header}>
      <Link href="/" className={styles.logo}>
        TechCart
      </Link>

      <SearchBar />

      <nav
        className={styles.menu}
        aria-label="Ana menü"
      >
        {/*
          localStorage kontrolü tamamlandıktan sonra
          kullanıcının oturum durumuna uygun menü gösterilir.
        */}
        {!isAuthLoading && (
          <>
            {isAuthenticated && user ? (
              /*
                Kullanıcı giriş yaptıysa Giriş Yap ve
                Kayıt Ol yerine Hesabım menüsü gösterilir.
              */
              <div
                className={styles.accountMenu}
                ref={accountMenuRef}
              >
                <button
                  className={styles.accountButton}
                  type="button"
                  aria-haspopup="menu"
                  aria-expanded={isAccountMenuOpen}
                  onClick={() =>
                    setIsAccountMenuOpen(
                      (currentValue) =>
                        !currentValue
                    )
                  }
                >
                  <UserCircle
                    size={20}
                    strokeWidth={1.8}
                  />

                  <span>Hesabım</span>

                  <ChevronDown
                    className={
                      isAccountMenuOpen
                        ? styles.openChevron
                        : styles.chevron
                    }
                    size={17}
                    strokeWidth={2}
                  />
                </button>

                {isAccountMenuOpen && (
                  <div
                    className={styles.accountDropdown}
                    role="menu"
                  >
                    {/* Giriş yapan kullanıcının kısa bilgisi */}
                    <div className={styles.userInformation}>
                      <strong>
                        {user.firstName} {user.lastName}
                      </strong>

                      <span>{user.email}</span>
                    </div>

                    <div
                      className={styles.dropdownSeparator}
                    />

                    <Link
                      href="/profile"
                      className={styles.dropdownLink}
                      role="menuitem"
                    >
                      <User size={18} />
                      Profilim
                    </Link>

                    <Link
                      href="/addresses"
                      className={styles.dropdownLink}
                      role="menuitem"
                    >
                      <MapPin size={18} />
                      Adreslerim
                    </Link>

                    <Link
                      href="/orders"
                      className={styles.dropdownLink}
                      role="menuitem"
                    >
                      <Package size={18} />
                      Siparişlerim
                    </Link>

                    <div
                      className={styles.dropdownSeparator}
                    />

                    <button
                      className={styles.logoutButton}
                      type="button"
                      role="menuitem"
                      onClick={handleLogout}
                    >
                      <LogOut size={18} />
                      Çıkış Yap
                    </button>
                  </div>
                )}
              </div>
            ) : (
              /*
                Kullanıcı giriş yapmadıysa giriş ve
                kayıt bağlantılarını gösteririz.
              */
              <>
                <Link
                  href="/login"
                  className={styles.loginLink}
                >
                  <User
                    size={20}
                    strokeWidth={1.8}
                  />
                  <span>Giriş Yap</span>
                </Link>

                <Link
                  href="/register"
                  className={styles.registerLink}
                >
                  <UserPlus
                    size={20}
                    strokeWidth={1.8}
                  />
                  <span>Kayıt Ol</span>
                </Link>
              </>
            )}
          </>
        )}

        {/* Sepet bağlantısı her iki durumda da görünür. */}
        <Link
          className={styles.cartLink}
          href="/cart"
          aria-label={`Sepetim, ${totalQuantity} ürün`}
        >
          Sepetim

          {totalQuantity > 0 && (
            <span
              className={styles.cartQuantity}
              aria-live="polite"
            >
              {totalQuantity}
            </span>
          )}
        </Link>
      </nav>
    </header>
  );
}