// teslimat adresinde tutulacak temel bilgiler
export type Address = {
  id: string;
  title: string;
  firstName: string;
  lastName: string;
  phone: string;
  city: string;
  district: string;
  neighborhood: string;
  addressLine: string;
  postalCode: string;
  isDefault: boolean; // varsayilan teslimat adresi mi
};