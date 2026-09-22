export type CartLineResponse = {
  productId: string;
  name: string;
  model: string;
  brand: string;
  category: string;
  image: string | null;
  unitPrice: number;
  vatRate: number;
  quantity: number;
  lineTotal: number;
  vatAmount: number;
  stock: number;
  inStock: boolean;
};

export type CartResponse = {
  items: CartLineResponse[];
  totalQuantity: number;
  subtotal: number;
  vatTotal: number;
  shippingFee: number;
  total: number;
};