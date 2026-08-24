import { Product } from "./product";

export type CartItem = {
  product: Product;
  quantity: number; // urunden kac adet eklendi
};