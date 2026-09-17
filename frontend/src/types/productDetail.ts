// GET /api/products/{id}'nin döndürdüğü şekille birebir eşleşen tip.
export type ProductDetail = {
  id: string;
  name: string;
  brand: string;
  category: string;
  model: string;
  description: string;
  specs: string; // serileştirilmiş JSON metni — şu an tüm ürünlerde "{}"
  price: number; // KDV dahil
  priceWithoutVat: number;
  vatRate: number; // ham oran (0.20 gibi)
  vatAmount: number;
  stock: number;
  inStock: boolean;
  cartQuantity: number;
  isReadyToShip: boolean;
  hasFastDelivery: boolean;
  images: string[];
};
