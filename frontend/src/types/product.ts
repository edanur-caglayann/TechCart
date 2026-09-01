
//Ürün kartında gösterilecek görsel türlerini tanımlar.
export type ProductVisualType =
  | "phone"
  | "computer"
  | "tablet"
  | "television"
  | "headphone"
  | "turntable";

/*
  Bir teknik özelliğin hangi alanlardan
  oluşacağını tanımlar.
*/
export type TechnicalSpecification = {
  label: string; // ozelligin adi
  value: string; // ozelligin degeri
};

/*
  Ürün veri yapısını tanımlar.Bu tip, ürün kartında ve ürün detay
  sayfasında kullanılacak bilgileri içerir.
*/
export type Product = {
  id: number;
  name: string;
  model: string;
  brand: string;
  category: string;
  price: number; // urunun kdv dahil satis fiyati
  vatRate: number; // urune uygulanacak olan kdv orani
  inStock: boolean;

  //gorsel kullanilana kadar urun icin kullanilan geçici ikon 
  visualType: ProductVisualType;
  description?: string;
  // urunun birden fazla gorseli olacak
  images?: string[];

  //Ürünün teknik özelliklerini liste hâlinde tutar.
  technicalSpecifications?: TechnicalSpecification[];

  //Ürünün mevcut stok miktarı
  stockQuantity?: number;
};