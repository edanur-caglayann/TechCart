/*
  React Hook Form, AuthContext ve state kullanıldığı
  için Profilim sayfası Client Component'tir.
*/
"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import {
  KeyRound,
  Mail,
  Pencil,
  Save,
  UserRound,
  X,
} from "lucide-react";
import { useRouter } from "next/navigation";
import {
  useEffect,
  useState,
} from "react";
import { useForm } from "react-hook-form";

import { useAuth } from "../../context/AuthContext";
import {
  changePasswordSchema,
  profileSchema,
  type ChangePasswordFormValues,
  type ProfileFormValues,
} from "../../schemas/authSchemas";

import styles from "./page.module.css";

import Link from "next/link";

export default function ProfilePage() {
  const router = useRouter();

  const {
    user,
    isAuthLoading,
    updateProfile,
  } = useAuth();

  // Profil ve parola formlarının açık olup olmadığını tutar.
  const [isEditingProfile, setIsEditingProfile] =
    useState(false);

  const [isChangingPassword, setIsChangingPassword] =
    useState(false);

  // Başarılı işlem mesajlarını tutar.
  const [profileMessage, setProfileMessage] =
    useState("");

  const [passwordMessage, setPasswordMessage] =
    useState("");

  // Ad, soyad ve e-posta formunu yönetir.
  const {
    register: registerProfile,
    handleSubmit: handleProfileSubmit,
    reset: resetProfile,
    setError: setProfileError,
    formState: {
      errors: profileErrors,
      isSubmitting: isProfileSubmitting,
    },
  } = useForm<ProfileFormValues>({
    resolver: zodResolver(profileSchema),
    defaultValues: {
      firstName: "",
      lastName: "",
      email: "",
    },
    mode: "onBlur",
  });

  // Parola değiştirme formunu yönetir.
  const {
    register: registerPassword,
    handleSubmit: handlePasswordSubmit,
    reset: resetPassword,
    setError: setPasswordError,
    formState: {
      errors: passwordErrors,
      isSubmitting: isPasswordSubmitting,
    },
  } = useForm<ChangePasswordFormValues>({
    resolver: zodResolver(changePasswordSchema),
    defaultValues: {
      currentPassword: "",
      newPassword: "",
      confirmNewPassword: "",
    },
    mode: "onBlur",
  });

  /*
    Oturum kontrolü tamamlandığında kullanıcı yoksa giriş sayfasına yönlendirir.
  */
  useEffect(() => {
    if (!isAuthLoading && !user) {
      router.replace("/login");
    }
  }, [isAuthLoading, user, router]);

  /*
    Kullanıcı bilgileri hazır olduğunda profil formunun başlangıç değerlerini doldurur.
  */
  useEffect(() => {
    if (user) {
      resetProfile({
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
      });
    }
  }, [user, resetProfile]);

  // Profil bilgilerini AuthContext üzerinden günceller.
  async function onProfileSubmit(
    formValues: ProfileFormValues
  ) {
    try {
      await updateProfile(formValues);

      setIsEditingProfile(false);
      setProfileMessage(
        "Profil bilgileriniz güncellendi."
      );
    } catch {
      setProfileError("root", {
        message:
          "Profil bilgileri güncellenemedi.",
      });
    }
  }

  /*
    Şimdilik yalnızca arayüz davranışını tamamlar.
    Backend hazır olduğunda parola değiştirme API isteği bu fonksiyon içerisinde yapılacak
  */
  async function onPasswordSubmit(
    _formValues: ChangePasswordFormValues
  ) {
    try {
      await Promise.resolve();

      resetPassword();
      setIsChangingPassword(false);
      setPasswordMessage(
        "Parolanız başarıyla güncellendi."
      );
    } catch {
      setPasswordError("root", {
        message: "Parola güncellenemedi.",
      });
    }
  }

  // Profil düzenleme işlemini iptal eder.
  function cancelProfileEditing() {
    if (user) {
      resetProfile({
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
      });
    }

    setIsEditingProfile(false);
  }

  // Parola değiştirme işlemini iptal eder.
  function cancelPasswordEditing() {
    resetPassword();
    setIsChangingPassword(false);
  }

  // Oturum kontrolü tamamlanana kadar sayfayı göstermez.
  if (isAuthLoading || !user) {
    return (
      <main className={styles.profilePage}>
        <p className={styles.loadingText}>
          Profil bilgileri yükleniyor...
        </p>
      </main>
    );
  }
return (
  <main className={styles.profilePage}>
    <div className={styles.profileContainer}>
  {/* Logo ve bölüm etiketi aynı satırda gösterilir */}
  <div className={styles.topBar}>
    <Link href="/" className={styles.logo}>
      TECHCART
    </Link>

    <span className={styles.sectionLabel}>
      HESAP AYARLARI
    </span>
  </div>

  <header className={styles.pageHeader}>
    <h1>Profilim</h1>
          <p>
            Kişisel bilgilerinizi ve parolanızı
            buradan yönetebilirsiniz.
          </p>
        </header>

        {/* Kişisel bilgiler bölümü */}
        <section className={styles.profileCard}>
          <div className={styles.cardHeader}>
            <div className={styles.cardTitle}>
              <UserRound size={22} />

              <div>
                <h2>Kişisel Bilgiler</h2>
                <p>Ad, soyad ve e-posta bilgileriniz.</p>
              </div>
            </div>

            {!isEditingProfile && (
              <button
                className={styles.editButton}
                type="button"
                onClick={() => {
                  setProfileMessage("");
                  setIsEditingProfile(true);
                }}
              >
                <Pencil size={17} />
                Düzenle
              </button>
            )}
          </div>

          {profileMessage && (
            <p className={styles.successMessage}>
              {profileMessage}
            </p>
          )}

          {isEditingProfile ? (
            <form
              className={styles.form}
              onSubmit={handleProfileSubmit(
                onProfileSubmit
              )}
              noValidate
            >
              <div className={styles.formRow}>
                <div className={styles.formGroup}>
                  <label htmlFor="firstName">Ad</label>

                  <input
                    id="firstName"
                    type="text"
                    autoComplete="given-name"
                    {...registerProfile("firstName")}
                  />

                  {profileErrors.firstName && (
                    <p className={styles.errorMessage}>
                      {profileErrors.firstName.message}
                    </p>
                  )}
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="lastName">
                    Soyad
                  </label>

                  <input
                    id="lastName"
                    type="text"
                    autoComplete="family-name"
                    {...registerProfile("lastName")}
                  />

                  {profileErrors.lastName && (
                    <p className={styles.errorMessage}>
                      {profileErrors.lastName.message}
                    </p>
                  )}
                </div>
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="email">
                  E-posta adresi
                </label>

                <div className={styles.inputWithIcon}>
                  <Mail size={18} />

                  <input
                    id="email"
                    type="email"
                    autoComplete="email"
                    {...registerProfile("email")}
                  />
                </div>

                {profileErrors.email && (
                  <p className={styles.errorMessage}>
                    {profileErrors.email.message}
                  </p>
                )}
              </div>

              {profileErrors.root && (
                <p className={styles.formError}>
                  {profileErrors.root.message}
                </p>
              )}

              <div className={styles.formActions}>
                <button
                  className={styles.cancelButton}
                  type="button"
                  onClick={cancelProfileEditing}
                >
                  <X size={17} />
                  İptal
                </button>

                <button
                  className={styles.saveButton}
                  type="submit"
                  disabled={isProfileSubmitting}
                >
                  <Save size={17} />
                  Değişiklikleri Kaydet
                </button>
              </div>
            </form>
          ) : (
            <div className={styles.informationGrid}>
              <div>
                <span>Ad</span>
                <strong>{user.firstName}</strong>
              </div>

              <div>
                <span>Soyad</span>
                <strong>{user.lastName || "-"}</strong>
              </div>

              <div>
                <span>E-posta</span>
                <strong>{user.email}</strong>
              </div>
            </div>
          )}
        </section>

        {/* Parola bölümü */}
        <section className={styles.profileCard}>
          <div className={styles.cardHeader}>
            <div className={styles.cardTitle}>
              <KeyRound size={22} />

              <div>
                <h2>Parola</h2>
                <p>Hesap parolanızı değiştirebilirsiniz.</p>
              </div>
            </div>

            {!isChangingPassword && (
              <button
                className={styles.editButton}
                type="button"
                onClick={() => {
                  setPasswordMessage("");
                  setIsChangingPassword(true);
                }}
              >
                <Pencil size={17} />
                Değiştir
              </button>
            )}
          </div>

          {passwordMessage && (
            <p className={styles.successMessage}>
              {passwordMessage}
            </p>
          )}

          {isChangingPassword ? (
            <form
              className={styles.form}
              onSubmit={handlePasswordSubmit(
                onPasswordSubmit
              )}
              noValidate
            >
              <div className={styles.formGroup}>
                <label htmlFor="currentPassword">
                  Mevcut parola
                </label>

                <input
                  id="currentPassword"
                  type="password"
                  autoComplete="current-password"
                  {...registerPassword(
                    "currentPassword"
                  )}
                />

                {passwordErrors.currentPassword && (
                  <p className={styles.errorMessage}>
                    {
                      passwordErrors.currentPassword
                        .message
                    }
                  </p>
                )}
              </div>

              <div className={styles.formRow}>
                <div className={styles.formGroup}>
                  <label htmlFor="newPassword">
                    Yeni parola
                  </label>

                  <input
                    id="newPassword"
                    type="password"
                    autoComplete="new-password"
                    {...registerPassword("newPassword")}
                  />

                  {passwordErrors.newPassword && (
                    <p className={styles.errorMessage}>
                      {passwordErrors.newPassword.message}
                    </p>
                  )}
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="confirmNewPassword">
                    Yeni parola tekrarı
                  </label>

                  <input
                    id="confirmNewPassword"
                    type="password"
                    autoComplete="new-password"
                    {...registerPassword(
                      "confirmNewPassword"
                    )}
                  />

                  {passwordErrors.confirmNewPassword && (
                    <p className={styles.errorMessage}>
                      {
                        passwordErrors
                          .confirmNewPassword.message
                      }
                    </p>
                  )}
                </div>
              </div>

              <p className={styles.passwordInformation}>
                Parola en az 8 karakter, büyük harf,
                küçük harf, rakam ve özel karakter
                içermelidir.
              </p>

              {passwordErrors.root && (
                <p className={styles.formError}>
                  {passwordErrors.root.message}
                </p>
              )}

              <div className={styles.formActions}>
                <button
                  className={styles.cancelButton}
                  type="button"
                  onClick={cancelPasswordEditing}
                >
                  <X size={17} />
                  İptal
                </button>

                <button
                  className={styles.saveButton}
                  type="submit"
                  disabled={isPasswordSubmitting}
                >
                  <Save size={17} />
                  Parolayı Güncelle
                </button>
              </div>
            </form>
          ) : (
            <div className={styles.passwordPreview}>
              <span>Mevcut parola</span>
              <strong>••••••••</strong>
            </div>
          )}
        </section>
      </div>
    </main>
  );
}