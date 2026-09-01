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

/* AddressContext üzerinden paylaşılacak değerler. */
type AddressContextType = {
  addresses: Address[];
  isAddressLoading: boolean;
  saveAddress: (address: Address) => void;
  makeDefault: (addressId: string) => void;
  deleteAddress: (addressId: string) => void;
};

const AddressContext = createContext<
  AddressContextType | undefined
>(undefined);

type AddressProviderProps = {
  children: ReactNode;
};

export function AddressProvider({
  children,
}: AddressProviderProps) {
  const { user } = useAuth();

  // Giriş yapan kullanıcının adreslerini tutar.
  const [addresses, setAddresses] = useState<
    Address[]
  >([]);

  // Adreslerin tarayıcıdan yüklenme durumunu tutar.
  const [
    isAddressLoading,
    setIsAddressLoading,
  ] = useState(true);

  // Kullanıcıya özel localStorage anahtarını tutar.
  const [storageKey, setStorageKey] = useState<
    string | null
  >(null);

  /*
    Kullanıcı giriş yaptığında kendisine ait kayıtlı adresleri localStorage'dan yükler.
  */
  useEffect(() => {
    if (!user) {
      setAddresses([]);
      setStorageKey(null);
      setIsAddressLoading(false);
      return;
    }

    setIsAddressLoading(true);

    const userStorageKey =
      `techcart-addresses-${user.email}`;

    const savedAddresses =
      localStorage.getItem(userStorageKey);

    if (savedAddresses) {
      try {
        const parsedAddresses: Address[] =
          JSON.parse(savedAddresses);

        setAddresses(parsedAddresses);
      } catch {
        // Kayıtlı adres verisi bozuksa temizler.
        localStorage.removeItem(userStorageKey);
        setAddresses([]);
      }
    } else {
      setAddresses([]);
    }

    setStorageKey(userStorageKey);
    setIsAddressLoading(false);
  }, [user]);

  /*
    Adres listesi değiştiğinde güncel listeyi localStorage içerisine kaydeder.
  */
  useEffect(() => {
    if (!storageKey || isAddressLoading) {
      return;
    }

    localStorage.setItem(
      storageKey,
      JSON.stringify(addresses)
    );
  }, [
    addresses,
    storageKey,
    isAddressLoading,
  ]);

  // Yeni adres ekler veya mevcut adresi günceller.
  function saveAddress(savedAddress: Address) {
    setAddresses((currentAddresses) => {
      const addressExists =
        currentAddresses.some(
          (address) =>
            address.id === savedAddress.id
        );

      let updatedAddresses = addressExists
        ? currentAddresses.map((address) =>
            address.id === savedAddress.id
              ? savedAddress
              : address
          )
        : [...currentAddresses, savedAddress];

      /*
        Kaydedilen adres varsayılansa diğer adreslerin varsayılan özelliğini kaldırır.
      */
      if (
        savedAddress.isDefault ||
        currentAddresses.length === 0
      ) {
        updatedAddresses = updatedAddresses.map(
          (address) => ({
            ...address,
            isDefault:
              address.id === savedAddress.id,
          })
        );
      }

      return updatedAddresses;
    });
  }

  // Seçilen adresi varsayılan adres yapar.
  function makeDefault(addressId: string) {
    setAddresses((currentAddresses) =>
      currentAddresses.map((address) => ({
        ...address,
        isDefault: address.id === addressId,
      }))
    );
  }

  // Seçilen adresi listeden kaldırır.
  function deleteAddress(addressId: string) {
    setAddresses((currentAddresses) => {
      const deletedAddress =
        currentAddresses.find(
          (address) => address.id === addressId
        );

      const remainingAddresses =
        currentAddresses.filter(
          (address) => address.id !== addressId
        );

      /*
        Varsayılan adres silindiyse kalan ilk adresi varsayılan olarak belirler.
      */
      if (
        deletedAddress?.isDefault &&
        remainingAddresses.length > 0
      ) {
        return remainingAddresses.map(
          (address, index) => ({
            ...address,
            isDefault: index === 0,
          })
        );
      }

      return remainingAddresses;
    });
  }

  return (
    <AddressContext.Provider
      value={{
        addresses,
        isAddressLoading,
        saveAddress,
        makeDefault,
        deleteAddress,
      }}
    >
      {children}
    </AddressContext.Provider>
  );
}

/* Adres bilgilerine ve fonksiyonlarına ulaşmayı sağlar. */

export function useAddress() {
  const context = useContext(AddressContext);

  if (!context) {
    throw new Error(
      "useAddress, AddressProvider içerisinde kullanılmalıdır."
    );
  }

  return context;
}