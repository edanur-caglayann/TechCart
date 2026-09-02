import type { Address } from "./address";

// Siparişin sahip olabileceği durumları tanımlar.
export type OrderStatus =
  | "Preparing"
  | "Shipped"
  | "Delivered";

// Sipariş içerisindeki ürünün anlık bilgilerini tutar.
export type OrderItem = {
  productId: number;
  name: string;
  model: string;
  brand: string;
  category: string;
  quantity: number;
  unitPrice: number;
  vatRate: number;
  totalPrice: number;
};

// Oluşturulan siparişin veri yapısını tanımlar.
export type Order = {
  id: string;
  orderNumber: string;
  userEmail: string;
  items: OrderItem[];
  deliveryAddress: Address;
  status: OrderStatus;
  totalQuantity: number;
  totalNetPrice: number;
  totalVat: number;
  totalPrice: number;
  createdAt: string;
};