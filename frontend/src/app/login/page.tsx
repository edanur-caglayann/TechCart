/*
  React Hook Form, AuthContext ve useRouter tarayıcı
  tarafında çalıştığı için bu sayfa Client Component'tir.
*/
"use client";

import {
  ArrowLeft,
  LockKeyhole,
  Mail,
} from "lucide-react";

import { zodResolver } from "@hookform/resolvers/zod";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";

import { useAuth } from "../../context/AuthContext";
import {
  loginSchema,
  LoginFormValues,
} from "../../schemas/authSchemas";

import {
  useEffect,
  useState,
} from "react";

import styles from "./page.module.css";

export default function LoginPage() {

  // Giriş başarılı olduğunda kullanıcıyı ana  sayfaya yönlendirmek için router kullanılır.
  const router = useRouter();
  /*
  Giriş tamamlandıktan sonra kullanıcının
  yönlendirileceği adresi tutar. Varsayılan yönlendirme ana sayfadır.
  */
  const [returnUrl, setReturnUrl] =
    useState("/");

  // Sayfa adresindeki returnUrl bilgisini okur.
  useEffect(() => {
    const searchParameters =
      new URLSearchParams(window.location.search);

    const requestedReturnUrl =
      searchParameters.get("returnUrl");

    // Yalnızca uygulama içerisindeki güvenli dreslerin kullanılmasına izin veririz.
    if (
      requestedReturnUrl &&
      requestedReturnUrl.startsWith("/") &&
      !requestedReturnUrl.startsWith("//")
    ) {
      setReturnUrl(requestedReturnUrl);
    }
  }, []);
  const { login: loginUser } = useAuth();

  /*
    React Hook Form kutuph. ile giriş alanlarını ve form durumunu yönetirirz.
    zodResolver ile girilen değerleri loginSchema kurallarıyla kontrol ederiz
  */
  const {
    register,
    handleSubmit,
    setError,
    formState: {
      errors,
      isSubmitting,
    },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),

    /* Form ilk açıldığında iki alan da boş olur. */
    defaultValues: {
      email: "",
      password: "",
    },
    mode: "onBlur",
  });

  async function onSubmit(
    formValues: LoginFormValues
  ) {
    try {
      //Formdan gelen giris bilgilerini AuthContext içerisindeki login fonksiyonuna göndeririz.
      await loginUser({
        email: formValues.email,
        password: formValues.password,
      });

      /*
       returnUrl varsa kullanıcı ilgili sayfaya,
       yoksa ana sayfaya yönlendirilir.
      */
      router.push(returnUrl);
    } catch {

      setError("root", {
        message:
          "E-posta veya parola hatalı. Lütfen tekrar deneyin.",
      });
    }
  }

  return (
    <main className={styles.loginPage}>
      <div className={styles.loginContainer}>
        {/* Sol taraftaki tanıtım alanı */}
        <section className={styles.introduction}>
          <Link className={styles.logo} href="/">
            TECHCART
          </Link>

          <div className={styles.introductionContent}>
            <span className={styles.label}>
              GÜVENLİ ALIŞVERİŞ
            </span>

            <h1>
              Alışverişine kaldığın yerden devam et.
            </h1>

            <p>
              Hesabına giriş yaparak sepetini
              yönetebilir, siparişlerini takip edebilir
              ve teslimat adreslerine ulaşabilirsin.
            </p>
          </div>

          {/* Kullanıcıyı ana sayfaya yönlendirir. */}
          <Link className={styles.backLink} href="/">
            <ArrowLeft size={18} />
            Ana sayfaya dön
          </Link>
        </section>

        {/* Sağ taraftaki giriş formu */}
        <section className={styles.formSection}>
          <div className={styles.formHeader}>
            <h2>Giriş Yap</h2>

            <p>
              TechCart hesabına erişmek için
              bilgilerini gir.
            </p>
          </div>

          {/*
            handleSubmit önce Zod doğrulamasını çalıştırır.
            noValidate ile tarayıcının varsayılan hata
            mesajları yerine kendi mesajlarımızı gösteririz.
          */}
          <form
            className={styles.loginForm}
            onSubmit={handleSubmit(onSubmit)}
            noValidate
          >
            <div className={styles.formGroup}>
              <label htmlFor="email">
                E-posta adresi
              </label>

              <div
                className={`${styles.inputWrapper} ${errors.email
                  ? styles.inputError
                  : ""
                  }`}
              >
                <Mail
                  className={styles.inputIcon}
                  size={20}
                  aria-hidden="true"
                />

                {/*
                  register("email"), bu inputu React Hook
                  Form içerisindeki email alanına bağlar.
                */}
                <input
                  id="email"
                  type="email"
                  placeholder="ornek@techcart.com"
                  autoComplete="email"
                  aria-invalid={Boolean(errors.email)}
                  aria-describedby={
                    errors.email
                      ? "email-error"
                      : undefined
                  }
                  {...register("email")}
                />
              </div>

              {/* E-posta doğrulama hatasını gösterir. */}
              {errors.email && (
                <p
                  id="email-error"
                  className={styles.errorMessage}
                  role="alert"
                >
                  {errors.email.message}
                </p>
              )}
            </div>

            {/*
              Parola başlığını, Parolamı Unuttum
              bağlantısını ve inputu aynı grupta tutar.
            */}
            <div className={styles.formGroup}>
              <div className={styles.passwordHeader}>
                <label htmlFor="password">
                  Parola
                </label>

                <Link href="/forgot-password">
                  Parolamı unuttum
                </Link>
              </div>

              <div
                className={`${styles.inputWrapper} ${errors.password
                  ? styles.inputError
                  : ""
                  }`}
              >
                <LockKeyhole
                  className={styles.inputIcon}
                  size={20}
                  aria-hidden="true"
                />

                <input
                  id="password"
                  type="password"
                  placeholder="Parolanı gir"
                  autoComplete="current-password"
                  aria-invalid={
                    Boolean(errors.password)
                  }
                  aria-describedby={
                    errors.password
                      ? "password-error"
                      : undefined
                  }
                  {...register("password")}
                />
              </div>

              {/* Parola doğrulama hatasını gösterir. */}
              {errors.password && (
                <p
                  id="password-error"
                  className={styles.errorMessage}
                  role="alert"
                >
                  {errors.password.message}
                </p>
              )}
            </div>

            {/* Giriş işleminden gelen genel hatayı gösterir. */}
            {errors.root && (
              <p
                className={styles.formError}
                role="alert"
              >
                {errors.root.message}
              </p>
            )}

            <button
              className={styles.loginButton}
              type="submit"
              disabled={isSubmitting}
            >
              {isSubmitting
                ? "Giriş yapılıyor..."
                : "Giriş Yap"}
            </button>
          </form>

          <div className={styles.registerArea}>
            <span>Henüz hesabın yok mu?</span>

            <Link
              href={`/register?returnUrl=${encodeURIComponent(
                returnUrl
              )}`}
            >
              Kayıt Ol
            </Link>
          </div>
        </section>
      </div>
    </main>
  );
}