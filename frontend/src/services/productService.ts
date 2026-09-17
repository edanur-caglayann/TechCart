import { apiFetch } from "./apiClient";
import type { ProductListItem } from "../types/productListItem";
import type { ProductDetail } from "../types/productDetail";

// Filtre/sıralama/sayfalama parametrelerinin tümü — Filters.tsx ve
// ProductList.tsx bu tipi kullanarak backend'e ne göndereceğini kurar.
export type ProductFilters = {
  q?: string;
  category?: string;
  brand?: string;
  minPrice?: number;
  maxPrice?: number;
  color?: string;
  inStock?: boolean;
  sortBy?: string;
  page?: number;
  pageSize?: number;
};

type PaginationInfo = { page: number; pageSize: number; totalItems: number; totalPages: number };
type ProductListResponse = { items: ProductListItem[]; pagination: PaginationInfo };

export type FacetOption = { id: string; name: string; count: number };
export type ColorFacetOption = { color: string; count: number };
export type ProductFacets = {
  categories: FacetOption[];
  brands: FacetOption[];
  colors: ColorFacetOption[];
  minPrice: number;
  maxPrice: number;
};

export type Suggestion = { text: string; type: "product" | "category" | "brand" };

// Boş/undefined alanları hiç eklemeden query string kurar — backend'in
// isteğe bağlı parametreleri gereksiz yere "boş string" olarak almasını önler.
function buildQueryString(filters: Record<string, string | number | boolean | undefined>): string {
  const params = new URLSearchParams();

  for (const [key, value] of Object.entries(filters)) {
    if (value !== undefined && value !== "") {
      params.append(key, String(value));
    }
  }

  const query = params.toString();
  return query ? `?${query}` : "";
}

export function listProductsRequest(filters: ProductFilters): Promise<ProductListResponse> {
  return apiFetch<ProductListResponse>(`/api/products${buildQueryString(filters)}`, { method: "GET" });
}

// sortBy/page/pageSize facet hesabını etkilemiyor (backend'de de böyleydi), bilerek göndermiyoruz.
export function getProductFacetsRequest(filters: Omit<ProductFilters, "sortBy" | "page" | "pageSize">): Promise<ProductFacets> {
  return apiFetch<ProductFacets>(`/api/products/facets${buildQueryString(filters)}`, { method: "GET" });
}

export function getProductSuggestionsRequest(term: string): Promise<Suggestion[]> {
  return apiFetch<Suggestion[]>(`/api/products/suggestions${buildQueryString({ q: term })}`, { method: "GET" });
}

export function getProductDetailRequest(id: string): Promise<ProductDetail> {
  return apiFetch<ProductDetail>(`/api/products/${id}`, { method: "GET" });
}