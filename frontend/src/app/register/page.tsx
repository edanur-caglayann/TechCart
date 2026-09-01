/*
  React Hook Form, AuthContext ve useRouter tarayıcı
  tarafında çalıştığı için bu sayfa Client Component'tir.
*/
"use client";

import {
  ArrowLeft,
  LockKeyhole,
  Mail,
  ShieldCheck,
  UserRound,
} from "lucide-react";

import { zodResolver } from "@hookform/resolvers/zod";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";

import { useAuth } from "../../context/AuthContext";
import {
  registerSchema,
  RegisterFormValues,
} from "../../schemas/authSchemas";

import {
  useEffect,
  useState,
} from "react";

import styles from "./page.module.css";

export default function RegisterPage() {
  // Kayıt tamamlandığında kullanıcıyı başka bir adrese yönlendirmek için router kullanılır.
  const router = useRouter();
  /*
  Kayıt tamamlandıktan sonra kullanıcının yönlendirileceği adresi tutar.
*/
  const [returnUrl, setReturnUrl] =
    useState("/");

  useEffect(() => {
    const searchParameters =
      new URLSearchParams(window.location.search);

    const requestedReturnUrl =
      searchParameters.get("returnUrl");

    // sadece uygulama içindeki adreslere izin verir.
    if (
      requestedReturnUrl &&
      requestedReturnUrl.startsWith("/") &&
      !requestedReturnUrl.startsWith("//")
    ) {
      setReturnUrl(requestedReturnUrl);
    }
  }, []);
  const { register: registerUser } = useAuth();

  /*
    React Hook Form form alanlarını ve form durumunu yönetir.
    zodResolver, girilen değerleri registerSchema ile kontrol eder.
  */
  const {
    register,
    handleSubmit,
    setError,
    formState: {
      errors,
      isSubmitting,
    },

    /* useForm, reacr hook form kutuph. form surecini yonetmek icin sundugu hook'tur
    React Hook Form input değerlerini toplar, zodRegister bu degerleri registerShema semsina gonderir
    zod tum kurallari kontrol eder
    hata varsa kutuphanenin errors nesnesine aktarililir
    hata yoksa onSubmit fonks calistirilir
     */
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),

    //Form ilk açıldığında alanların sahip olacağı başlangıç değerlerini 
    defaultValues: {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
      acceptTerms: false,
    },

    // Kullanıcı input alanından çıktığında ilgili alanın doğrulama kontrolü çalışır.
    mode: "onBlur",
  });

  // Form, Zod kontrollerinden başarıyla gectiginde calisa fonk
  async function onSubmit(
    formValues: RegisterFormValues
  ) {
    try {
      await registerUser({
        firstName: formValues.firstName,
        lastName: formValues.lastName,
        email: formValues.email,
        password: formValues.password,
      });

      // returnUrl varsa ilgili sayfaya, yoksa ana sayfaya gider.
      router.push(returnUrl);
    } catch {

      //Kayıt işlemi başarısız olursa formun üstünde genel bir hata mesajı gösterilir
      setError("root", {
        message:
          "Kayıt işlemi tamamlanamadı. Lütfen tekrar deneyin.",
      });
    }
  }

  return (
    <main className={styles.registerPage}>
      <div className={styles.registerContainer}>
        {/* Sol taraftaki üyelik tanıtım alanı */}
        <section className={styles.introduction}>
          <Link className={styles.logo} href="/">
            TECHCART
          </Link>

          <div className={styles.introductionContent}>
            <span className={styles.label}>
              TECHCART ÜYELİĞİ
            </span>

            <h1>
              Alışveriş deneyimini kişiselleştir.
            </h1>

            <p>
              Hesabını oluştur, ürünleri sepetine
              ekle, siparişlerini takip et ve teslimat
              adreslerini kolayca yönet.
            </p>

            <ul className={styles.benefitList}>
              <li>
                <ShieldCheck size={20} />
                Güvenli hesap ve sipariş yönetimi
              </li>

              <li>
                <ShieldCheck size={20} />
                Sepet ve sipariş geçmişine erişim
              </li>

              <li>
                <ShieldCheck size={20} />
                Teslimat adreslerini kolayca yönetme
              </li>
            </ul>
          </div>

          <Link className={styles.backLink} href="/">
            <ArrowLeft size={18} />
            Ana sayfaya dön
          </Link>
        </section>

        {/* Sağ taraftaki kayıt formu */}
        <section className={styles.formSection}>
          <div className={styles.formHeader}>
            <h2>Hesap Oluştur</h2>

            <p>
              TechCart’a kayıt olmak için bilgilerini gir.
            </p>
          </div>

          {/*
            noValidate, tarayıcının varsayılan hata mesajlarını
            kapatır. Hataları Zod üzerinden biz gösteririz.
          */}
          <form
            className={styles.registerForm}
            onSubmit={handleSubmit(onSubmit)}
            noValidate
          >
            <div className={styles.formRow}>
              <div className={styles.formGroup}>
                <label htmlFor="firstName">
                  Ad
                </label>

                <div
                  className={`${styles.inputWrapper} ${errors.firstName
                    ? styles.inputError
                    : ""
                    }`}
                >
                  <UserRound
                    className={styles.inputIcon}
                    size={20}
                    aria-hidden="true"
                  />

                  {/*
                    register("firstName"), input değerini
                    React Hook Form'a bağlar.
                  */}
                  <input
                    id="firstName"
                    type="text"
                    placeholder="Adını gir"
                    autoComplete="given-name"
                    aria-invalid={
                      Boolean(errors.firstName)
                    }
                    aria-describedby={
                      errors.firstName
                        ? "firstName-error"
                        : undefined
                    }
                    {...register("firstName")}
                  />
                </div>

                {errors.firstName && (
                  <p
                    id="firstName-error"
                    className={styles.errorMessage}
                    role="alert"
                  >
                    {errors.firstName.message}
                  </p>
                )}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="lastName">
                  Soyad
                </label>

                <div
                  className={`${styles.inputWrapper} ${errors.lastName
                    ? styles.inputError
                    : ""
                    }`}
                >
                  <UserRound
                    className={styles.inputIcon}
                    size={20}
                    aria-hidden="true"
                  />

                  <input
                    id="lastName"
                    type="text"
                    placeholder="Soyadını gir"
                    autoComplete="family-name"
                    aria-invalid={
                      Boolean(errors.lastName)
                    }
                    aria-describedby={
                      errors.lastName
                        ? "lastName-error"
                        : undefined
                    }
                    {...register("lastName")}
                  />
                </div>

                {errors.lastName && (
                  <p
                    id="lastName-error"
                    className={styles.errorMessage}
                    role="alert"
                  >
                    {errors.lastName.message}
                  </p>
                )}
              </div>
            </div>

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

            <div className={styles.formRow}>
              <div className={styles.formGroup}>
                <label htmlFor="password">
                  Parola
                </label>

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
                    autoComplete="new-password"
                    aria-invalid={
                      Boolean(errors.password)
                    }
                    aria-describedby={
                      errors.password
                        ? "password-error"
                        : "password-information"
                    }
                    {...register("password")}
                  />
                </div>

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

              <div className={styles.formGroup}>
                <label htmlFor="confirmPassword">
                  Parola tekrarı
                </label>

                <div
                  className={`${styles.inputWrapper} ${errors.confirmPassword
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
                    id="confirmPassword"
                    type="password"
                    placeholder="Parolanı tekrar gir"
                    autoComplete="new-password"
                    aria-invalid={Boolean(
                      errors.confirmPassword
                    )}
                    aria-describedby={
                      errors.confirmPassword
                        ? "confirmPassword-error"
                        : undefined
                    }
                    {...register("confirmPassword")}
                  />
                </div>

                {errors.confirmPassword && (
                  <p
                    id="confirmPassword-error"
                    className={styles.errorMessage}
                    role="alert"
                  >
                    {errors.confirmPassword.message}
                  </p>
                )}
              </div>
            </div>

            <p
              id="password-information"
              className={styles.passwordInformation}
            >
              Parola en az 8 karakter, büyük harf,
              küçük harf, rakam ve özel karakter
              içermelidir.
            </p>

            <label className={styles.termsArea}>
              <input
                type="checkbox"
                {...register("acceptTerms")}
              />

              <span>
                <Link href="/kullanim-kosullari">
                  Kullanım Koşulları
                </Link>

                {" ve "}

                <Link href="/gizlilik-politikasi">
                  Gizlilik Politikası
                </Link>

                ’nı okudum ve kabul ediyorum.
              </span>
            </label>

            {errors.acceptTerms && (
              <p
                className={styles.errorMessage}
                role="alert"
              >
                {errors.acceptTerms.message}
              </p>
            )}

            {errors.root && (
              <p
                className={styles.formError}
                role="alert"
              >
                {errors.root.message}
              </p>
            )}

            <button
              className={styles.registerButton}
              type="submit"
              disabled={isSubmitting}
            >
              {isSubmitting
                ? "Hesap oluşturuluyor..."
                : "Hesap Oluştur"}
            </button>
          </form>

          <div className={styles.loginArea}>
            <span>Zaten hesabın var mı?</span>

            <Link
              href={`/login?returnUrl=${encodeURIComponent(
                returnUrl
              )}`}
            >
              Giriş Yap
            </Link>
          </div>
        </section>
      </div>
    </main>
  );
}