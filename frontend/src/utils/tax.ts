// Ürünlerde kullanılacak varsayılan KDV oranı.
export const DEFAULT_VAT_RATE = 20;

// Para değerini iki ondalık basamağa yuvarlar.
function roundMoney(amount: number) {
  return Math.round((amount + Number.EPSILON) * 100) / 100;
}

// KDV dahil fiyatın içerisindeki KDV tutarını hesaplar.
export function calculateIncludedVat(
  grossAmount: number,
  vatRate: number = DEFAULT_VAT_RATE
) {
  const vatAmount =
    (grossAmount * vatRate) / (100 + vatRate);

  return roundMoney(vatAmount);
}

// KDV dahil fiyatın KDV hariç tutarını hesaplar.
export function calculateNetAmount(
  grossAmount: number,
  vatRate: number = DEFAULT_VAT_RATE
) {
  const vatAmount = calculateIncludedVat(
    grossAmount,
    vatRate
  );

  return roundMoney(grossAmount - vatAmount);
}

// Fiyatı Türk lirası biçiminde gösterir.
export function formatCurrency(amount: number) {
  return new Intl.NumberFormat("tr-TR", {
    style: "currency",
    currency: "TRY",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount);
}