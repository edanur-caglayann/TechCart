/* Urun veri yapisini tanimlar. Urun hangi bilgileri tasimasi gerektigi belirtilir. */
export type ProductVisualType = 
  | "phone"
  | "computer"
  | "tablet"
  | "television"
  | "headphone"
  | "turntable";

  export type Product = {
    id: number;
    name: string;
    model: string;
    brand: string; // marka
    category: string;
    rating: number; // kullanicinin verdigi puan 
    reviewCount: number; // kac kullanici puan verdi
    price: number;
    inStock: boolean;
    visualType: ProductVisualType; // ikon
    // imageUrl: string;
  };