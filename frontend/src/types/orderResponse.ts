export type OrderItemLineResponse = {
  productId: string | null;
  productName: string;
  productModel: string;
  quantity: number;
  unitPrice: number;
  vatRate: number;
  vatAmount: number;
};

export type OrderPaymentSummaryResponse = {
  provider: string;
  maskedCardNumber: string | null;
  paidAt: string;
};

export type OrderResponse = {
  id: string;
  orderNumber: string;
  status: string;
  createdAt: string;
  subtotal: number;
  vatTotal: number;
  shippingFee: number;
  total: number;
  shippingFullName: string;
  shippingPhone: string;
  shippingCity: string;
  shippingDistrict: string;
  shippingAddressLine: string;
  shippingPostalCode: string;
  payment: OrderPaymentSummaryResponse | null;
  items: OrderItemLineResponse[];
};

export type CheckoutInitResponse = {
  paymentPageUrl: string;
  token: string;
};