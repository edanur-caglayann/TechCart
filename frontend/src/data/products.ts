/* Urunleri ortak bir dosyada tutuyoruz
ProductList -> urunleri listelemek icin
Urun detay sayfasi -> secilen urunu bulmak icin
bu diziyi kullanir.
*/
import type { Product } from "../types/product";
import { DEFAULT_VAT_RATE } from "../utils/tax";

/*
  Ürün verisinde KDV oranı yazılmazsa
  varsayılan oran kullanılabilir.
*/
type ProductData = Omit<Product, "vatRate"> & {
  vatRate?: number;
};

const productData: ProductData[] = [
{
  id: 1,
  name: "iPhone 15 Pro",
  model: "A3101",
  brand: "Apple",
  category: "Telefon",
  price: 89999,
  vatRate: 18,
  inStock: true,
  visualType: "phone",
  description:
    "iPhone 15 Pro; güçlü işlemcisi, gelişmiş kamera sistemi ve dayanıklı titanyum tasarımıyla yüksek performans sunar.",
  stockQuantity: 8,
  images: [
  "/products/iphone-15-pro/iphone-front.png",
  "/products/iphone-15-pro/iphone-back.webp",
  "/products/iphone-15-pro/iphone-color.webp",
  ],

  // Ürün detay sayfasında gösterilecek teknik özelliklerin listesi
  technicalSpecifications: [
    {
      label: "Ekran",
      value: "6.1 inç OLED",
    },
    {
      label: "İşlemci",
      value: "A17 Pro",
    },
    {
      label: "Depolama",
      value: "256 GB",
    },
    {
      label: "Kamera",
      value: "48 MP",
    },
    {
      label: "Renk",
      value: "Siyah Titanyum",
    },
    {
      label: "Bağlantı",
      value: "5G",
    },
  ],
},
  {
    id: 2,
    name: "Galaxy S24 Ultra",
    model: "SM-S928B",
    brand: "Samsung",
    category: "Telefon",
    price: 74999,
    inStock: true,
    visualType: "phone",
  },
  {
    id: 3,
    name: "MacBook Pro 14",
    model: "M3 Pro",
    brand: "Apple",
    category: "Bilgisayar",
    price: 129999,
    inStock: true,
    visualType: "computer",
  },
  {
    id: 4,
    name: "ROG Zephyrus G16",
    model: "GU605",
    brand: "Asus",
    category: "Bilgisayar",
    price: 79999,
    inStock: false,
    visualType: "computer",
  },
  {
    id: 5,
    name: "OLED evo Smart TV",
    model: "OLED55C4",
    brand: "LG",
    category: "Televizyon",
    price: 64999,
    inStock: true,
    visualType: "television",
  },
  {
    id: 6,
    name: "iPad Air",
    model: "M2 11 inç",
    brand: "Apple",
    category: "Tablet",
    price: 39999,
    inStock: true,
    visualType: "tablet",
  },
  {
    id: 7,
    name: "Kablosuz Kulaklık",
    model: "WH-1000XM5",
    brand: "Sony",
    category: "Aksesuar",
    price: 14999,
    inStock: true,
    visualType: "headphone",
  },
  {
    id: 8,
    name: "Tam Otomatik Pikap",
    model: "AT-LP60X",
    brand: "Audio-Technica",
    category: "Pikap",
    price: 12999,
    inStock: true,
    visualType: "turntable",
  },
    {
    id: 9,
    name: "Galaxy Tab S9",
    model: "SM-X710",
    brand: "Samsung",
    category: "Tablet",
    price: 32999,
    inStock: true,
    visualType: "tablet",
  },
  {
    id: 10,
    name: "Bravia 4K Smart TV",
    model: "KD-55X85L",
    brand: "Sony",
    category: "Televizyon",
    price: 57999,
    inStock: true,
    visualType: "television",
  },
  {
    id: 11,
    name: "IdeaPad Slim 5",
    model: "14IRL8",
    brand: "Lenovo",
    category: "Bilgisayar",
    price: 42999,
    inStock: true,
    visualType: "computer",
  },
  {
    id: 12,
    name: "AirPods Pro",
    model: "2. Nesil",
    brand: "Apple",
    category: "Aksesuar",
    price: 11999,
    inStock: false,
    visualType: "headphone",
  },
  {
    id: 13,
    name: "Xiaomi 14 Ultra",
    model: "24030PN60G",
    brand: "Xiaomi",
    category: "Telefon",
    price: 69999,
    inStock: true,
    visualType: "phone",
  },
  {
    id: 14,
    name: "OLED Smart TV",
    model: "55S90D",
    brand: "Samsung",
    category: "Televizyon",
    price: 84999,
    inStock: true,
    visualType: "television",
  },
  {
    id: 15,
    name: "Zenbook 14 OLED",
    model: "UX3405",
    brand: "Asus",
    category: "Bilgisayar",
    price: 64999,
    inStock: true,
    visualType: "computer",
  },
  {
    id: 16,
    name: "Bluetooth Pikap",
    model: "PS-LX310BT",
    brand: "Sony",
    category: "Pikap",
    price: 15999,
    inStock: true,
    visualType: "turntable",
  },
    {
    id: 17,
    name: "Tune 770NC Kablosuz Kulaklık",
    model: "JBLT770NCBLK",
    brand: "JBL",
    category: "Aksesuar",
    price: 4999,
    vatRate: 25,
    inStock: true,
    stockQuantity: 12,
    visualType: "headphone",
    description:
      "Aktif gürültü engelleme özelliği ve uzun pil ömrüyle kablosuz müzik deneyimi sunar.",
    technicalSpecifications: [
      {
        label: "Bağlantı",
        value: "Bluetooth 5.3",
      },
      {
        label: "Pil Ömrü",
        value: "70 saate kadar",
      },
      {
        label: "Gürültü Engelleme",
        value: "Aktif",
      },
      {
        label: "Renk",
        value: "Siyah",
      },
    ],
  },
];
/*
  KDV oranı belirtilmeyen bütün ürünlere varsayılan KDV oranını otomatik olarak ekler.
*/
export const products: Product[] = productData.map(
  (product) => ({
    ...product,
    vatRate:
      product.vatRate ?? DEFAULT_VAT_RATE,
  })
);