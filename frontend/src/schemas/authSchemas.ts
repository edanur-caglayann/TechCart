import { z } from "zod"; // z-> zod kutuphanesinden aktardigimiz nesne


//Kayıt formundaki tum alanların doğrulama kurallarını tek bir yerde tanımlarız.
export const registerSchema = z 
  .object({
    firstName: z
      .string()
      .trim() // metnin basindakki ve sonundaki bosluklari kaldirir
      .min(1, "Ad alanı boş bırakılamaz.") // min karakter sayisi, kural saglanmazsa gostreilecek olan hata mesaji
      .min(2, "Ad en az 2 karakter olmalıdır."),

    lastName: z
      .string()
      .trim()
      .min(1, "Soyad alanı boş bırakılamaz.")
      .min(2, "Soyad en az 2 karakter olmalıdır."),

    email: z
      .string()
      .trim()
      .min(1, "E-posta alanı boş bırakılamaz.")
      .email("Geçerli bir e-posta adresi giriniz."),

    password: z
      .string()
      .min(1, "Parola alanı boş bırakılamaz.")
      .min(8, "Parola en az 8 karakter olmalıdır.")
      .regex( // regular expression -> duzenli ifade
        /[A-Z]/,
        "Parola en az bir büyük harf içermelidir."
      )
      .regex( // ilgili karakter yapisi bulunuyor mu kontrolu
        /[a-z]/,
        "Parola en az bir küçük harf içermelidir."
      )
      .regex(
        /[0-9]/,
        "Parola en az bir rakam içermelidir."
      )
      .regex(
        /[^A-Za-z0-9\s]/, // bunlar diisnda !?@ gibi ozel karakter
        "Parola en az bir özel karakter içermelidir."
      ),

    confirmPassword: z
      .string()
      .min(1, "Parola tekrarı boş bırakılamaz."),

    acceptTerms: z.boolean().refine(
      (isAccepted) => isAccepted,
      {
        message:
          "Kullanım koşullarını kabul etmelisiniz.",
      }
    ),
  })

  // İki parolanin aynı olup olmadığı kontrolu
  .refine(
    (formValues) =>
      formValues.password ===
      formValues.confirmPassword,
    {
      message: "Parolalar eşleşmiyor.",
      path: ["confirmPassword"],
    }
  );

/*
  Form verisinin TypeScript tipini Zod şemasından
  otomatik olarak oluştururuz.
*/
export type RegisterFormValues = z.infer<
  typeof registerSchema
>;


// Giriş formunda e-posta ve parola alanlarına uygulanacak doğrulama kurallarını tanımlarız.
export const loginSchema = z.object({
  email: z
    .string()
    .trim()
    .min(1, "E-posta alanı boş bırakılamaz.")
    .email("Geçerli bir e-posta adresi giriniz."),

  password: z
    .string()
    .min(1, "Parola alanı boş bırakılamaz."),
});

export type LoginFormValues = z.infer<
  typeof loginSchema
>;

// Profil bilgilerinin doğrulama kuralları.
export const profileSchema = z.object({
  firstName: z
    .string()
    .trim()
    .min(1, "Ad alanı boş bırakılamaz.")
    .min(2, "Ad en az 2 karakter olmalıdır."),

  lastName: z
    .string()
    .trim()
    .min(1, "Soyad alanı boş bırakılamaz.")
    .min(2, "Soyad en az 2 karakter olmalıdır."),

  email: z
    .string()
    .trim()
    .min(1, "E-posta alanı boş bırakılamaz.")
    .email("Geçerli bir e-posta adresi giriniz."),
});

export type ProfileFormValues = z.infer<
  typeof profileSchema
>;

// Parola değiştirme formunun doğrulama kuralları.
export const changePasswordSchema = z
  .object({
    currentPassword: z
      .string()
      .min(1, "Mevcut parola boş bırakılamaz."),

    newPassword: z
      .string()
      .min(1, "Yeni parola boş bırakılamaz.")
      .min(8, "Yeni parola en az 8 karakter olmalıdır.")
      .regex(
        /[A-Z]/,
        "Yeni parola en az bir büyük harf içermelidir."
      )
      .regex(
        /[a-z]/,
        "Yeni parola en az bir küçük harf içermelidir."
      )
      .regex(
        /[0-9]/,
        "Yeni parola en az bir rakam içermelidir."
      )
      .regex(
        /[^A-Za-z0-9\s]/,
        "Yeni parola en az bir özel karakter içermelidir."
      ),

    confirmNewPassword: z
      .string()
      .min(
        1,
        "Yeni parola tekrarı boş bırakılamaz."
      ),
  })
  // Yeni parola ile parola tekrarının eşleşme kontrolü.
  .refine(
    (formValues) =>
      formValues.newPassword ===
      formValues.confirmNewPassword,
    {
      message: "Yeni parolalar eşleşmiyor.",
      path: ["confirmNewPassword"],
    }
  );

export type ChangePasswordFormValues = z.infer<
  typeof changePasswordSchema
>;