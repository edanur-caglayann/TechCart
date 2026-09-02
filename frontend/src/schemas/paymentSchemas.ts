import { z } from "zod";

/*
  Kart numarasının artan veya azalan
  sıralı rakamlardan oluşup oluşmadığını kontrol eder.
*/
function isSequentialCardNumber(
  cardNumber: string
) {
  const digits = cardNumber.replace(/\s/g, "");

  if (digits.length < 2) {
    return false;
  }

  let isIncreasing = true;
  let isDecreasing = true;

  for (
    let index = 1;
    index < digits.length;
    index++
  ) {
    const previousDigit = Number(
      digits[index - 1]
    );

    const currentDigit = Number(
      digits[index]
    );

    if (
      currentDigit !==
      (previousDigit + 1) % 10
    ) {
      isIncreasing = false;
    }

    if (
      currentDigit !==
      (previousDigit + 9) % 10
    ) {
      isDecreasing = false;
    }
  }

  return isIncreasing || isDecreasing;
}

/*
  Aynı rakam grubunun sürekli tekrarlandığı
  kart numaralarını kontrol eder.
*/
function hasRepeatedPattern(
  cardNumber: string
) {
  const digits = cardNumber.replace(/\s/g, "");

  if (digits.length < 2) {
    return false;
  }

  for (
    let patternLength = 1;
    patternLength <= digits.length / 2;
    patternLength++
  ) {
    let isRepeated = true;

    for (
      let index = patternLength;
      index < digits.length;
      index++
    ) {
      if (
        digits[index] !==
        digits[index % patternLength]
      ) {
        isRepeated = false;
        break;
      }
    }

    if (isRepeated) {
      return true;
    }
  }

  return false;
}

// Kart bilgilerinin doğrulama kurallarını tanımlar.
export const paymentSchema = z.object({
  cardHolder: z
    .string()
    .trim()
    .min(
      1,
      "Kart üzerindeki ad soyad boş bırakılamaz."
    )
    .min(3, "Geçerli bir ad soyad giriniz."),

  cardNumber: z
    .string()
    .trim()
    .min(1, "Kart numarası boş bırakılamaz.")
    /*
      Boşluklar kaldırıldıktan sonra kart
      numarasının tam 16 rakam olmasını ister.
    */
    .refine(
      (cardNumber) => {
        const digits = cardNumber.replace(
          /\s/g,
          ""
        );

        return /^\d{16}$/.test(digits);
      },
      {
        message:
          "Kart numarası 16 rakam olmalıdır.",
      }
    )
    /*
      1111111111111111 gibi yalnızca aynı
      rakamdan oluşan numaraları kabul etmez.
    */
    .refine(
      (cardNumber) => {
        const digits = cardNumber.replace(
          /\s/g,
          ""
        );

        return !/^(\d)\1{15}$/.test(digits);
      },
      {
        message:
          "Aynı rakamdan oluşan kart numarası kullanılamaz.",
      }
    )
    /*
      1234123412341234 gibi aynı rakam
      grubunun tekrarlandığı numaraları kabul etmez.
    */
    .refine(
      (cardNumber) =>
        !hasRepeatedPattern(cardNumber),
      {
        message:
          "Tekrarlanan rakam gruplarından oluşan kart numarası kullanılamaz.",
      }
    )
    /*
      1234567890123456 gibi belirgin şekilde
      sıralanan kart numaralarını kabul etmez.
    */
    .refine(
      (cardNumber) =>
        !isSequentialCardNumber(cardNumber),
      {
        message:
          "Sıralı rakamlardan oluşan kart numarası kullanılamaz.",
      }
    ),

  expiryDate: z
    .string()
    .trim()
    .min(
      1,
      "Son kullanma tarihi boş bırakılamaz."
    )
    .regex(
      /^(0[1-9]|1[0-2])\/\d{2}$/,
      "Son kullanma tarihini AA/YY biçiminde giriniz."
    )
    .refine(
      (expiryDate) => {
        const [month, year] =
          expiryDate.split("/");

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
        message:
          "Kartın son kullanma tarihi geçmiş.",
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

  useThreeDSecure: z
    .boolean()
    .refine(
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