import { apiFetch } from "./apiClient";
import type { Address } from "../types/address";

/*
  Backend'in gerçek şekli — frontend'in Address tipinden İKİ yerde farklı:
  firstName+lastName yerine tek fullName var. Bu farkı burada, servis
  katmanında kapatıyoruz; Context ve sayfa kodu hep frontend'in kendi
  Address tipiyle çalışmaya devam ediyor, backend'in şeklini hiç bilmiyor.
*/
type BackendAddressDto = {
  id: string;
  title: string;
  fullName: string;
  phone: string;
  city: string;
  district: string;
  neighborhood: string;
  addressLine: string;
  postalCode: string;
  isDefault: boolean;
};

type AddressPayload = {
  title: string;
  fullName: string;
  phone: string;
  city: string;
  district: string;
  neighborhood: string;
  addressLine: string;
  postalCode: string;
};

/* Backend'den gelen tek bir adresi frontend'in Address şekline çevirir. */
function toFrontendAddress(dto: BackendAddressDto): Address {

  const [firstWord, ...rest] = dto.fullName.trim().split(" ");

  return {
    id: dto.id,
    title: dto.title,
    firstName: firstWord ?? "",
    lastName: rest.join(" "),
    phone: dto.phone,
    city: dto.city,
    district: dto.district,
    neighborhood: dto.neighborhood,
    addressLine: dto.addressLine,
    postalCode: dto.postalCode,
    isDefault: dto.isDefault,
  };
}

/* Frontend'in adres verisini backend'e giden payload şekline çevirir. */
function toBackendPayload(address: Omit<Address, "id" | "isDefault">): AddressPayload {
  return {
    title: address.title,
    fullName: `${address.firstName.trim()} ${address.lastName.trim()}`.trim(),
    phone: address.phone,
    city: address.city,
    district: address.district,
    neighborhood: address.neighborhood,
    addressLine: address.addressLine,
    postalCode: address.postalCode,
  };
}

export async function listAddressesRequest(): Promise<Address[]> {
  const result = await apiFetch<BackendAddressDto[]>("/api/users/me/addresses", {
    method: "GET",
    requiresAuth: true,
  });
  return result.map(toFrontendAddress);
}

export async function createAddressRequest(address: Omit<Address, "id" | "isDefault">): Promise<Address> {
  const result = await apiFetch<BackendAddressDto>("/api/users/me/addresses", {
    method: "POST",
    body: toBackendPayload(address),
    requiresAuth: true,
  });
  return toFrontendAddress(result);
}

export async function updateAddressRequest(addressId: string, address: Omit<Address, "id" | "isDefault">): Promise<Address> {
  const result = await apiFetch<BackendAddressDto>(`/api/users/me/addresses/${addressId}`, {
    method: "PATCH",
    body: toBackendPayload(address),
    requiresAuth: true,
  });
  return toFrontendAddress(result);
}

export async function deleteAddressRequest(addressId: string): Promise<void> {
  await apiFetch<void>(`/api/users/me/addresses/${addressId}`, {
    method: "DELETE",
    requiresAuth: true,
  });
}

export async function setDefaultAddressRequest(addressId: string): Promise<Address[]> {
  const result = await apiFetch<BackendAddressDto[]>(`/api/users/me/addresses/${addressId}/default`, {
    method: "PATCH",
    requiresAuth: true,
  });
  return result.map(toFrontendAddress);
}