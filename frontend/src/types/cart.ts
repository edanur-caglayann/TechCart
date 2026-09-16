import type { ProductListItem } from "./productListItem";

export type CartItem = {
  product: ProductListItem;
  quantity: number;
};