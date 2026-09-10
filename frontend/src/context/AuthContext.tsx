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

import {
  registerRequest,
  loginRequest,
  sessionRequest,
  logoutRequest,
  updateProfileRequest,
} from "../services/authService";

import { getToken, setToken, clearToken } from "../services/apiClient";

/* AuthContext üzerinden paylaşılacak değerler. */
type AuthContextType = {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isAuthLoading: boolean;

  register: (credentials: RegisterCredentials) => Promise<void>;
  login: (credentials: LoginCredentials) => Promise<void>;
  updateProfile: (profileData: UpdateProfileData) => Promise<void>;
  logout: () => Promise<void>; 
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

type AuthProviderProps = {
  children: ReactNode;
};

export function AuthProvider({ children }: AuthProviderProps) {
  /*
    user, giriş yapan kullanıcıyı tutar.
    Oturum yoksa null değerindedir.
  */
  const [user, setUser] = useState<AuthUser | null>(null);

  /*
    Sayfa ilk açıldığında token doğrulaması (GET /api/auth/session)
    tamamlanana kadar true değerindedir.
  */
  const [isAuthLoading, setIsAuthLoading] = useState(true);

  /*
    Sayfa ilk açıldığında localStorage'da token var mı bakılır.
    ARTIK kullanıcı objesi cache'lenmiyor — sadece token saklanıyor,
    kullanıcı bilgisi HER ZAMAN backend'den (GET /api/auth/session)
    doğrulanarak alınıyor. Böylece token süresi dolmuş/geçersizse
    (örn. şifre başka yerden değiştirilmişse) eski/yanlış bir
    kullanıcı state'te asılı kalmıyor.
  */
  useEffect(() => {
    async function restoreSession() {
      const token = getToken();

      if (!token) {
        setIsAuthLoading(false);
        return;
      }

      try {
        const session = await sessionRequest();
        setUser(session.user);
      } catch {
        /* Token geçersiz/süresi dolmuş: temizle, misafir durumuna düş. */
        clearToken();
        setUser(null);
      } finally {
        setIsAuthLoading(false);
      }
    }

    restoreSession();
  }, []);

  /*
    register ve login'in ortak yaptığı iş: backend'den dönen
    token'ı localStorage'a, kullanıcıyı state'e yazmak.
  */
  function persistSession(token: string, authUser: AuthUser) {
    setToken(token);
    setUser(authUser);
  }

  /*
    Yeni kullanıcı kaydını backend'e gönderir. Başarılı olursa
    backend login ile aynı formatta (token + user) döndüğü için
    kullanıcı otomatik giriş yapmış sayılır, ayrıca login çağrısı
    atmaya gerek yok.

    Hata durumunda (örn. EMAIL_ALREADY_EXISTS) ApiError fırlar,
    burada yakalamıyoruz — formun kendisi yakalayıp gösterecek.
  */
  async function register(credentials: RegisterCredentials) {
    const { token, user: authUser } = await registerRequest(credentials);
    persistSession(token, authUser);
  }

  /*
    E-posta ve parolayla giriş yapar. Hata durumunda
    (INVALID_CREDENTIALS) ApiError fırlar, formun kendisi yakalar.
  */
  async function login(credentials: LoginCredentials) {
    const { token, user: authUser } = await loginRequest(credentials);
    persistSession(token, authUser);
  }

  /*
    Profildeki ad, soyad ve e-posta bilgilerini backend'e günceller.
    Backend güncel kullanıcıyı döndürüyor, state'i onunla eşitliyoruz
    (kendimiz local olarak birleştirip tahmin etmiyoruz).
  */
  async function updateProfile(profileData: UpdateProfileData) {
    if (!user) {
      throw new Error("Profil güncellemek için giriş yapmalısınız.");
    }

    const updatedUser = await updateProfileRequest(profileData);
    setUser(updatedUser);
  }

  /*
    Çıkış yapar. Doküman gereği asıl işlem frontend'de state/localStorage
    temizliği — backend çağrısı best-effort: başarısız olsa bile
    (örn. backend o an erişilemezse) kullanıcıyı yine de local olarak
    çıkış yaptırıyoruz, kullanıcı "çıkış yapamadım" diye takılı kalmasın.
  */
  async function logout() {
    try {
      await logoutRequest();
    } catch {
      // Sessizce yut — local temizlik zaten aşağıda yapılacak.
    }

    setUser(null);
    clearToken();
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
    throw new Error("useAuth, AuthProvider içerisinde kullanılmalıdır.");
  }

  return context;
}