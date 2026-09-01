import { z } from "zod";

// Kart bilgilerinin doğrulama kurallarını tanımlar.
export const paymentSchema = z.object({
  cardHolder: z
    .string()
    .trim()
    .min(1, "Kart üzerindeki ad soyad boş bırakılamaz.")
    .min(3, "Geçerli bir ad soyad giriniz."),

  cardNumber: z
    .string()
    .trim()
    .min(1, "Kart numarası boş bırakılamaz.")
    .refine(
      (cardNumber) => {
        const digits = cardNumber.replace(/\s/g, "");
        return /^\d{13,19}$/.test(digits);
      },
      {
        message: "Geçerli bir kart numarası giriniz.",
      }
    ),

  expiryDate: z
    .string()
    .trim()
    .min(1, "Son kullanma tarihi boş bırakılamaz.")
    .regex(
      /^(0[1-9]|1[0-2])\/\d{2}$/,
      "Son kullanma tarihini AA/YY biçiminde giriniz."
    )
    .refine(
      (expiryDate) => {
        const [month, year] = expiryDate.split("/");
        const expiryMonth = new Date(
          2000 + Number(year),
          Number(month),
          1
        );
        const currentDate = new Date();
        const currentMonth = new Date(
          currentDate.getFullYear(),
          currentDate.getMonth(),
          1
        );

        return expiryMonth > currentMonth;
      },
      {
        message: "Kartın son kullanma tarihi geçmiş.",
      }
    ),

  cvv: z
    .string()
    .trim()
    .min(1, "CVV alanı boş bırakılamaz.")
    .regex(
      /^\d{3,4}$/,
      "CVV 3 veya 4 rakam olmalıdır."
    ),

  useThreeDSecure: z.boolean().refine(
    (isAccepted) => isAccepted,
    {
      message:
        "3D Secure doğrulamasını kabul etmelisiniz.",
    }
  ),
});

// Ödeme formunun TypeScript tipini şemadan oluşturur.
export type PaymentFormValues = z.infer<
  typeof paymentSchema
>;