"use client";

import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";

import { useAuth } from "./AuthContext";
import type { Address } from "../types/address";
import {
  listAddressesRequest,
  createAddressRequest,
  updateAddressRequest,
  deleteAddressRequest,
  setDefaultAddressRequest,
} from "../services/addressService";

/*
  Üç fonksiyon da Promise<void> döndürüyor — backend'e gerçek istek attıkları
  için asenkron. Çağıran kodda (addresses/page.tsx) await + try/catch kullanman gerekiyor.
*/
type AddressContextType = {
  addresses: Address[];
  isAddressLoading: boolean;
  saveAddress: (address: Address) => Promise<void>;
  makeDefault: (addressId: string) => Promise<void>;
  deleteAddress: (addressId: string) => Promise<void>;
};

const AddressContext = createContext<AddressContextType | undefined>(undefined);

type AddressProviderProps = {
  children: ReactNode;
};

export function AddressProvider({ children }: AddressProviderProps) {
  const { isAuthenticated } = useAuth();

  const [addresses, setAddresses] = useState<Address[]>([]);
  const [isAddressLoading, setIsAddressLoading] = useState(true);

  /*
    Giriş yapılmamışsa istek atmıyoruz.
  */
  useEffect(() => {
    if (!isAuthenticated) {
      setAddresses([]);
      setIsAddressLoading(false);
      return;
    }

    refreshAddresses();
  }, [isAuthenticated]);

  async function refreshAddresses() {
    setIsAddressLoading(true);
    try {
      const result = await listAddressesRequest();
      setAddresses(result);
    } finally {
      setIsAddressLoading(false);
    }
  }

  /*
    create/update sadece adresin kendi bilgisini kaydediyor; varsayılan
    işaretliyse AYRI bir istekle (setDefaultAddressRequest) işaretleniyor —
    backend'deki "aynı anda tek varsayılan" kuralı atomik kalsın diye.
  */
  async function saveAddress(address: Address) {
    const savedAddress = address.id
      ? await updateAddressRequest(address.id, address)
      : await createAddressRequest(address);

    if (address.isDefault) {
      // Bu istek zaten güncel TÜM listeyi döndürüyor, ekstra bir liste isteği atmıyoruz.
      const updatedList = await setDefaultAddressRequest(savedAddress.id);
      setAddresses(updatedList);
    } else {
      await refreshAddresses();
    }
  }

  async function makeDefault(addressId: string) {
    const updatedList = await setDefaultAddressRequest(addressId);
    setAddresses(updatedList);
  }

  /*
    Silinen adres varsayılansa hangi adresin yeni varsayılan olacağına backend
    karar veriyor (en eski kalan adres) — bu mantığı frontend'de tekrar
    üretmiyoruz, silme sonrası listeyi backend'den taze çekiyoruz.
  */
  async function deleteAddress(addressId: string) {
    await deleteAddressRequest(addressId);
    await refreshAddresses();
  }

  return (
    <AddressContext.Provider
      value={{ addresses, isAddressLoading, saveAddress, makeDefault, deleteAddress }}
    >
      {children}
    </AddressContext.Provider>
  );
}

export function useAddress() {
  const context = useContext(AddressContext);

  if (!context) {
    throw new Error("useAddress, AddressProvider içerisinde kullanılmalıdır.");
  }

  return context;
}