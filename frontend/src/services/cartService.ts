import { apiFetch } from "./apiClient";
import type { CartResponse } from "../types/cartResponse";

export function getCartRequest(): Promise<CartResponse> {
  return apiFetch<CartResponse>("/api/cart", { method: "GET", requiresAuth: true });
}

export function addCartItemRequest(productId: string, quantity: number): Promise<CartResponse> {
  return apiFetch<CartResponse>("/api/cart/add-item", {
    method: "POST",
    body: { productId, quantity },
    requiresAuth: true,
  });
}

export function updateCartItemQuantityRequest(productId: string, quantity: number): Promise<CartResponse> {
  return apiFetch<CartResponse>(`/api/cart/items/${productId}`, {
    method: "PATCH",
    body: { quantity },
    requiresAuth: true,
  });
}

export function removeCartItemRequest(productId: string): Promise<CartResponse> {
  return apiFetch<CartResponse>(`/api/cart/items/${productId}`, {
    method: "DELETE",
    requiresAuth: true,
  });
}

export function clearCartRequest(): Promise<CartResponse> {
  return apiFetch<CartResponse>("/api/cart/items", {
    method: "DELETE",
    requiresAuth: true,
  });
}

export function mergeCartRequest(items: { productId: string; quantity: number }[]): Promise<CartResponse> {
  return apiFetch<CartResponse>("/api/cart/merge", {
    method: "POST",
    body: { items },
    requiresAuth: true,
  });
}