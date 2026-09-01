/*
  useState, useEffect ve localStorage tarayıcı
  tarafında kullanıldığı için Client Component'tir.
*/

"use client";

import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";

import type {
  AuthUser,
  LoginCredentials,
  RegisterCredentials,
  UpdateProfileData,
} from "../types/auth";

/* Kullanıcı bilgisinin saklanacağı localStorage anahtarı. */
const AUTH_STORAGE_KEY = "techcart-user";

/* AuthContext üzerinden paylaşılacak değerler. */
type AuthContextType = {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isAuthLoading: boolean;

  register: (
    credentials: RegisterCredentials
  ) => Promise<void>;

  login: (
    credentials: LoginCredentials
  ) => Promise<void>;

  // Profildeki ad, soyad ve e-posta bilgilerini günceller.
  updateProfile: (
    profileData: UpdateProfileData
  ) => Promise<void>;

  logout: () => void;
};

const AuthContext = createContext<
  AuthContextType | undefined
>(undefined);

type AuthProviderProps = {
  children: ReactNode;
};

export function AuthProvider({
  children,
}: AuthProviderProps) {
  /*
    user, giriş yapan kullanıcıyı tutar.
    Oturum yoksa null değerindedir.
  */
  const [user, setUser] = useState<AuthUser | null>(
    null
  );

  /*
    Sayfa ilk açıldığında localStorage kontrolü
    tamamlanana kadar true değerindedir.
  */
  const [isAuthLoading, setIsAuthLoading] =
    useState(true);

  /*
    Sayfa ilk açıldığında daha önce kaydedilmiş
    kullanıcı bilgilerini localStorage üzerinden okur.
  */
  useEffect(() => {
    const savedUser = localStorage.getItem(
      AUTH_STORAGE_KEY
    );

    if (savedUser) {
      try {
        /*
          JSON.parse, localStorage içindeki JSON metnini
          tekrar kullanılabilir JavaScript nesnesine dönüştürür.
        */
        const parsedUser: AuthUser =
          JSON.parse(savedUser);

        setUser(parsedUser);
      } catch {
        /*
          Kayıtlı veri geçerli JSON değilse
          bozuk veriyi tarayıcıdan kaldırır.
        */
        localStorage.removeItem(AUTH_STORAGE_KEY);
      }
    }

    setIsAuthLoading(false);
  }, []);

  /*
    Kullanıcıyı state ve localStorage içerisine kaydeden
    ortak yardımcı fonksiyon.

    Register ve Login işlemlerinde aynı kodun
    tekrar yazılmasını engeller.
  */
  function persistUser(authUser: AuthUser) {
    setUser(authUser);

    localStorage.setItem(
      AUTH_STORAGE_KEY,
      JSON.stringify(authUser)
    );
  }

  /*
    Yeni kullanıcı kaydı tamamlandığında çalışır.
    Şimdilik backend bulunmadığı için geçici bir
    Customer kullanıcısı oluşturup oturumu başlatır.
  */
  async function register(
    credentials: RegisterCredentials
  ) {
    const authUser: AuthUser = {
      firstName: credentials.firstName,
      lastName: credentials.lastName,
      email: credentials.email,
      role: "Customer",
    };

    persistUser(authUser);
  }

  /*
    Giriş formundan e-posta ve parola bilgilerini alır.
    Şimdilik frontend testi için geçici kullanıcı oluşturur.

    Parola state veya localStorage içerisine kaydedilmez.
    Backend eklendiğinde bu fonksiyon API'den gelen
    kullanıcı bilgisini persistUser'a gönderecektir.
  */
  async function login(
    credentials: LoginCredentials
  ) {
    const nameFromEmail =
      credentials.email.split("@")[0];

    const authUser: AuthUser = {
      firstName: nameFromEmail,
      lastName: "",
      email: credentials.email,
      role: "Customer",
    };

    persistUser(authUser);
  }

  /*
    Profil sayfasından gelen ad, soyad ve e-posta
    bilgileriyle mevcut kullanıcıyı günceller.

    Şimdilik bilgiler state ve localStorage içerisinde
    güncellenir. daha sonra backend'de API
    isteği ile yapılacak
  */
  async function updateProfile(
    profileData: UpdateProfileData
  ) {
    // Giriş yapan kullanıcı yoksa güncelleme yapılamaz.
    if (!user) {
      throw new Error(
        "Profil güncellemek için giriş yapmalısınız."
      );
    }

    /*
      Mevcut kullanıcının rol bilgisini koruyup değiştirilebilir profil alanlarını yenileriz.
    */
    const updatedUser: AuthUser = {
      ...user,
      firstName: profileData.firstName,
      lastName: profileData.lastName,
      email: profileData.email,
    };

    /*
      Güncellenen kullanıcıyı hem React state'ine hem de localStorage içerisine kaydeder.
    */
    persistUser(updatedUser);
  }

  /*
    Kullanıcı çıkış yaptığında hem state'i hem de tarayıcıdaki kullanıcı bilgisini temizler.
  */
  function logout() {
    setUser(null);
    localStorage.removeItem(AUTH_STORAGE_KEY);
  }

  /*
    user null değilse kullanıcı giriş yapmıştır.
  */
  const isAuthenticated = user !== null;

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated,
        isAuthLoading,
        register,
        login,
        updateProfile,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

/*
  AuthContext değerlerine ulaşmak için kullanılan ortak hook.
*/
export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error(
      "useAuth, AuthProvider içerisinde kullanılmalıdır."
    );
  }

  return context;
}