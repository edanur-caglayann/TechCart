"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import {
  MapPin,
  Pencil,
  Plus,
  Save,
  Star,
  Trash2,
  X,
} from "lucide-react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import {
  useEffect,
  useState,
} from "react";
import { useForm } from "react-hook-form";

import { useAuth } from "../../context/AuthContext";
import {
  addressSchema,
  type AddressFormValues,
} from "../../schemas/addressSchemas";
import type { Address } from "../../types/address";

import styles from "./page.module.css";

export default function AddressesPage() {
  const router = useRouter();
  const {
    user,
    isAuthLoading,
  } = useAuth();

  // Kullanıcının adreslerini tutar.
  const [addresses, setAddresses] = useState<Address[]>(
    []
  );

  const [isFormOpen, setIsFormOpen] = useState(false);

  // Düzenlenen adresin kimliğini tutar.
  const [editingAddressId, setEditingAddressId] =
    useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: {
      errors,
      isSubmitting,
    },
  } = useForm<AddressFormValues>({
    resolver: zodResolver(addressSchema),
    defaultValues: {
      title: "",
      firstName: "",
      lastName: "",
      phone: "",
      city: "",
      district: "",
      neighborhood: "",
      addressLine: "",
      postalCode: "",
      isDefault: false,
    },
    mode: "onBlur",
  });

  // Kullanıcı giriş yapmamışsa giriş sayfasına gönderir.
  useEffect(() => {
    if (!isAuthLoading && !user) {
      router.replace("/login");
    }
  }, [isAuthLoading, user, router]);

  // Yeni adres formunu açar.
  function openNewAddressForm() {
    setEditingAddressId(null);

    reset({
      title: "",
      firstName: user?.firstName ?? "",
      lastName: user?.lastName ?? "",
      phone: "",
      city: "",
      district: "",
      neighborhood: "",
      addressLine: "",
      postalCode: "",
      isDefault: addresses.length === 0,
    });

    setIsFormOpen(true);
  }

  // Seçilen adresi düzenleme formuna aktarır.
  function openEditForm(address: Address) {
    setEditingAddressId(address.id);
    reset(address);
    setIsFormOpen(true);
  }

  // Formu kapatır ve temizler.
  function closeForm() {
    setEditingAddressId(null);
    setIsFormOpen(false);
    reset();
  }

  // Yeni adres ekler veya mevcut adresi günceller.
  function onSubmit(formValues: AddressFormValues) {
    const addressId =
      editingAddressId ?? crypto.randomUUID();

    const editedAddress = addresses.find(
      (address) => address.id === editingAddressId
    );

    const shouldBeDefault =
      addresses.length === 0 ||
      formValues.isDefault ||
      Boolean(editedAddress?.isDefault);

    const savedAddress: Address = {
      id: addressId,
      ...formValues,
      isDefault: shouldBeDefault,
    };

    setAddresses((currentAddresses) => {
      let updatedAddresses = editingAddressId
        ? currentAddresses.map((address) =>
            address.id === editingAddressId
              ? savedAddress
              : address
          )
        : [...currentAddresses, savedAddress];

      // Yeni varsayılan seçildiyse diğer adresleri normal yapar.
      if (shouldBeDefault) {
        updatedAddresses = updatedAddresses.map(
          (address) => ({
            ...address,
            isDefault: address.id === addressId,
          })
        );
      }

      return updatedAddresses;
    });

    closeForm();
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

  // Seçilen adresi siler.
  function deleteAddress(addressId: string) {
    const shouldDelete = window.confirm(
      "Bu adresi silmek istediğinize emin misiniz?"
    );

    if (!shouldDelete) {
      return;
    }

    setAddresses((currentAddresses) => {
      const deletedAddress = currentAddresses.find(
        (address) => address.id === addressId
      );

      const remainingAddresses =
        currentAddresses.filter(
          (address) => address.id !== addressId
        );

      // Varsayılan adres silinirse ilk adresi varsayılan yapar.
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

  if (isAuthLoading || !user) {
    return (
      <main className={styles.addressesPage}>
        <p className={styles.loadingText}>
          Adres bilgileri yükleniyor...
        </p>
      </main>
    );
  }

  return (
    <main className={styles.addressesPage}>
      <div className={styles.container}>
        <div className={styles.topBar}>
          <Link href="/" className={styles.logo}>
            TECHCART
          </Link>

          <span className={styles.sectionLabel}>
            ADRES YÖNETİMİ
          </span>
        </div>

        <header className={styles.pageHeader}>
          <div>
            <h1>Adreslerim</h1>
            <p>
              Teslimat adreslerinizi ekleyebilir ve
              yönetebilirsiniz.
            </p>
          </div>

          <button
            className={styles.addButton}
            type="button"
            onClick={openNewAddressForm}
          >
            <Plus size={18} />
            Yeni Adres Ekle
          </button>
        </header>

        {/* Yeni adres ekleme ve düzenleme formu */}
        {isFormOpen && (
          <section className={styles.formCard}>
            <div className={styles.formHeader}>
              <h2>
                {editingAddressId
                  ? "Adresi Düzenle"
                  : "Yeni Adres Ekle"}
              </h2>

              <button
                className={styles.closeButton}
                type="button"
                onClick={closeForm}
                aria-label="Formu kapat"
              >
                <X size={20} />
              </button>
            </div>

            <form
              className={styles.form}
              onSubmit={handleSubmit(onSubmit)}
              noValidate
            >
              <div className={styles.formRow}>
                <div className={styles.formGroup}>
                  <label htmlFor="title">
                    Adres başlığı
                  </label>

                  <input
                    id="title"
                    type="text"
                    placeholder="Ev, İş"
                    {...register("title")}
                  />

                  {errors.title && (
                    <p className={styles.errorMessage}>
                      {errors.title.message}
                    </p>
                  )}
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="phone">
                    Telefon
                  </label>

                  <input
                    id="phone"
                    type="tel"
                    placeholder="05XXXXXXXXX"
                    {...register("phone")}
                  />

                  {errors.phone && (
                    <p className={styles.errorMessage}>
                      {errors.phone.message}
                    </p>
                  )}
                </div>
              </div>

              <div className={styles.formRow}>
                <div className={styles.formGroup}>
                  <label htmlFor="firstName">Ad</label>

                  <input
                    id="firstName"
                    type="text"
                    {...register("firstName")}
                  />

                  {errors.firstName && (
                    <p className={styles.errorMessage}>
                      {errors.firstName.message}
                    </p>
                  )}
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="lastName">
                    Soyad
                  </label>

                  <input
                    id="lastName"
                    type="text"
                    {...register("lastName")}
                  />

                  {errors.lastName && (
                    <p className={styles.errorMessage}>
                      {errors.lastName.message}
                    </p>
                  )}
                </div>
              </div>

              <div className={styles.formRow}>
                <div className={styles.formGroup}>
                  <label htmlFor="city">Şehir</label>

                  <input
                    id="city"
                    type="text"
                    {...register("city")}
                  />

                  {errors.city && (
                    <p className={styles.errorMessage}>
                      {errors.city.message}
                    </p>
                  )}
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="district">İlçe</label>

                  <input
                    id="district"
                    type="text"
                    {...register("district")}
                  />

                  {errors.district && (
                    <p className={styles.errorMessage}>
                      {errors.district.message}
                    </p>
                  )}
                </div>
              </div>

              <div className={styles.formRow}>
                <div className={styles.formGroup}>
                  <label htmlFor="neighborhood">
                    Mahalle
                  </label>

                  <input
                    id="neighborhood"
                    type="text"
                    {...register("neighborhood")}
                  />

                  {errors.neighborhood && (
                    <p className={styles.errorMessage}>
                      {errors.neighborhood.message}
                    </p>
                  )}
                </div>

                <div className={styles.formGroup}>
                  <label htmlFor="postalCode">
                    Posta kodu
                  </label>

                  <input
                    id="postalCode"
                    type="text"
                    inputMode="numeric"
                    {...register("postalCode")}
                  />

                  {errors.postalCode && (
                    <p className={styles.errorMessage}>
                      {errors.postalCode.message}
                    </p>
                  )}
                </div>
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="addressLine">
                  Açık adres
                </label>

                <textarea
                  id="addressLine"
                  rows={4}
                  {...register("addressLine")}
                />

                {errors.addressLine && (
                  <p className={styles.errorMessage}>
                    {errors.addressLine.message}
                  </p>
                )}
              </div>

              <label className={styles.checkboxArea}>
                <input
                  type="checkbox"
                  {...register("isDefault")}
                />

                Varsayılan teslimat adresim olsun
              </label>

              <div className={styles.formActions}>
                <button
                  className={styles.cancelButton}
                  type="button"
                  onClick={closeForm}
                >
                  İptal
                </button>

                <button
                  className={styles.saveButton}
                  type="submit"
                  disabled={isSubmitting}
                >
                  <Save size={17} />

                  {editingAddressId
                    ? "Değişiklikleri Kaydet"
                    : "Adresi Kaydet"}
                </button>
              </div>
            </form>
          </section>
        )}

        {/* Kayıtlı adres listesi */}
        {addresses.length === 0 ? (
          <section className={styles.emptyState}>
            <MapPin size={34} />

            <h2>Kayıtlı adresiniz bulunmuyor</h2>

            <p>
              Siparişlerinizde kullanmak için yeni bir
              teslimat adresi ekleyebilirsiniz.
            </p>
          </section>
        ) : (
          <div className={styles.addressGrid}>
            {addresses.map((address) => (
              <article
                className={`${styles.addressCard} ${
                  address.isDefault
                    ? styles.defaultCard
                    : ""
                }`}
                key={address.id}
              >
                <div className={styles.cardHeader}>
                  <div className={styles.titleArea}>
                    <MapPin size={20} />
                    <h2>{address.title}</h2>
                  </div>

                  {address.isDefault && (
                    <span className={styles.defaultBadge}>
                      Varsayılan
                    </span>
                  )}
                </div>

                <strong>
                  {address.firstName} {address.lastName}
                </strong>

                <p className={styles.addressText}>
                  {address.neighborhood},{" "}
                  {address.addressLine}
                </p>

                <p className={styles.addressMeta}>
                  {address.district} / {address.city}
                  <br />
                  {address.postalCode}
                  <br />
                  {address.phone}
                </p>

                <div className={styles.cardActions}>
                  {!address.isDefault && (
                    <button
                      className={styles.defaultButton}
                      type="button"
                      onClick={() =>
                        makeDefault(address.id)
                      }
                    >
                      <Star size={16} />
                      Varsayılan Yap
                    </button>
                  )}

                  <button
                    className={styles.secondaryButton}
                    type="button"
                    onClick={() =>
                      openEditForm(address)
                    }
                  >
                    <Pencil size={16} />
                    Düzenle
                  </button>

                  <button
                    className={styles.deleteButton}
                    type="button"
                    onClick={() =>
                      deleteAddress(address.id)
                    }
                  >
                    <Trash2 size={16} />
                    Sil
                  </button>
                </div>
              </article>
            ))}
          </div>
        )}
      </div>
    </main>
  );
}