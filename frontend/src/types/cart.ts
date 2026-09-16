export type CartProduct = {
  id: string;
  name: string;
  model: string;
  brand: string;
  category: string;
  price: number;
  vatRate: number;
  image: string | null;
  inStock: boolean;
  stockQuantity?: number;
};

export type CartItem = {
  product: CartProduct;
  quantity: number;
};
