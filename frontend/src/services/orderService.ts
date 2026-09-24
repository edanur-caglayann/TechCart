import { apiFetch } from "./apiClient";
import type { CheckoutInitResponse, OrderResponse } from "../types/orderResponse";

export function createOrderRequest(addressId: string): Promise<OrderResponse> {
  return apiFetch<OrderResponse>("/api/orders", {
    method: "POST",
    body: { addressId },
    requiresAuth: true,
  });
}

export function initiateCheckoutRequest(orderId: string): Promise<CheckoutInitResponse> {
  return apiFetch<CheckoutInitResponse>(`/api/orders/${orderId}/checkout`, {
    method: "POST",
    requiresAuth: true,
  });
}

export function listOrdersRequest(): Promise<OrderResponse[]> {
  return apiFetch<OrderResponse[]>("/api/orders", {
    method: "GET",
    requiresAuth: true,
  });
}