
export type ProductListItem = {
  id: string;      // backend GUID kullanıyor, sayı değil
  name: string;
  brand: string;
  category: string;
  price: number;    // KDV dahil, backend'de hesaplanmış
  image: string | null;
  inStock: boolean;
};