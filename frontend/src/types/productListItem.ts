export type ProductListItem = {
  id: string;
  name: string;
  model: string;
  brand: string;
  category: string;
  price: number;
  vatRate: number;
  image: string | null;
  inStock: boolean;
};
