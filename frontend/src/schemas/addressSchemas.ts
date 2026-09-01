import { z } from "zod";

/*
  Yeni adres ekleme ve adres düzenleme formlarının doğrulama kuralları.
*/
export const addressSchema = z.object({
  title: z
    .string()
    .trim()
    .min(1, "Adres başlığı boş bırakılamaz.")
    .min(2, "Adres başlığı en az 2 karakter olmalıdır.")
    .max(30, "Adres başlığı en fazla 30 karakter olabilir."),

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

  phone: z
    .string()
    .trim()
    .min(1, "Telefon numarası boş bırakılamaz.")
    .regex(
      /^(?:\+90|0)?5\d{9}$/,
      "Geçerli bir telefon numarası giriniz."
    ),

  city: z
    .string()
    .trim()
    .min(1, "Şehir alanı boş bırakılamaz.")
    .min(2, "Şehir en az 2 karakter olmalıdır."),

  district: z
    .string()
    .trim()
    .min(1, "İlçe alanı boş bırakılamaz.")
    .min(2, "İlçe en az 2 karakter olmalıdır."),

  neighborhood: z
    .string()
    .trim()
    .min(1, "Mahalle alanı boş bırakılamaz.")
    .min(2, "Mahalle en az 2 karakter olmalıdır."),

  addressLine: z
    .string()
    .trim()
    .min(1, "Açık adres boş bırakılamaz.")
    .min(10, "Açık adres en az 10 karakter olmalıdır."),

  postalCode: z
    .string()
    .trim()
    .regex(
      /^\d{5}$/,
      "Posta kodu 5 rakamdan oluşmalıdır."
    ),

  // Adresin varsayılan teslimat adresi olup olmadığı.
  isDefault: z.boolean(),
});

/*
  Adres formunun TypeScript tipini Zod şemasından otomatik oluşturur.
*/
export type AddressFormValues = z.infer<
  typeof addressSchema
>;