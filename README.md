<div align="center">

# TechCart

**Modern ve ölçeklenebilir bir full-stack e-ticaret uygulaması**

Next.js • ASP.NET Core • PostgreSQL • Docker

</div>

---

## Proje Hakkında

TechCart; teknoloji ürünlerinin keşfedilmesi, karşılaştırılması ve satın alınması süreçlerini kullanıcı dostu bir deneyimle sunan full-stack bir e-ticaret uygulamasıdır.

Proje; katmanlı backend mimarisi, bağımsız frontend uygulaması, test altyapısı ve container tabanlı geliştirme ortamıyla yapılandırılmıştır.

## Proje Kapsamı

- Kullanıcı kaydı, giriş ve rol tabanlı yetkilendirme
- Ürün, kategori, marka ve stok yönetimi
- Ürün listeleme ve detay görüntüleme
- Arama, filtreleme ve sıralama
- Sepet ve ürün adedi yönetimi
- Ürün karşılaştırma
- Sipariş oluşturma ve durum takibi
- 3D Secure destekli test ödeme entegrasyonu
- Satın alınan ürünler için puanlama
- Yönetici işlemleri

## Teknoloji Yığını

| Alan | Teknoloji |
|---|---|
| Frontend | Next.js, React, TypeScript |
| Backend | ASP.NET Core Web API, .NET 9 |
| Veritabanı | PostgreSQL |
| Arama | Elasticsearch |
| Test | xUnit |
| Container | Docker, Docker Compose |

## Backend Mimarisi

Backend, sorumlulukların ayrılmasını sağlayan katmanlı mimari yaklaşımıyla yapılandırılmıştır.

| Katman | Sorumluluk |
|---|---|
| `TechCart.API` | HTTP istekleri ve API endpoint’leri |
| `TechCart.Application` | Servisler, DTO’lar ve kullanım senaryoları |
| `TechCart.Domain` | Entity’ler, enum’lar ve iş kuralları |
| `TechCart.Persistence` | PostgreSQL ve veri erişim işlemleri |
| `TechCart.Infrastructure` | Arama, ödeme ve e-posta entegrasyonları |

## Kurulum

### Gereksinimler

- Git
- Docker Desktop

### Repository’yi klonlama

```bash
git clone https://github.com/edanur-caglayann/TechCart.git
cd TechCart
```

### Docker ile çalıştırma

Docker Desktop’ın çalıştığından emin olduktan sonra:

```bash
docker compose up --build
```

Servis adresleri:

| Servis | Adres |
|---|---|
| Frontend | `http://localhost:3000` |
| Backend API | `http://localhost:8080` |
| PostgreSQL | `localhost:5433` |
| OpenAPI | `http://localhost:8080/openapi/v1.json` |

Projeyi arka planda çalıştırmak için:

```bash
docker compose up --build -d
```

Çalışan servisleri görüntülemek için:

```bash
docker compose ps
```

Projeyi durdurmak için:

```bash
docker compose down
```

## Docker Kullanmadan Çalıştırma

### Frontend

```bash
cd frontend
npm install
npm run dev
```

### Backend API

```bash
dotnet run --project backend/src/TechCart.API
```

## Testler

Bütün backend testlerini çalıştırmak için:

```bash
dotnet test backend/TechCart.sln
```

Yalnızca unit testleri çalıştırmak için:

```bash
cd tests/TechCart.UnitTests
dotnet test
```

> Solution dosyası `TechCart.slnx` biçimindeyse komutta `TechCart.sln` yerine `TechCart.slnx` kullanılmalıdır.

## Docker Yapısı

```text
TechCart Compose Project
├── techcart-frontend
├── techcart-api
└── techcart-postgres
```

Frontend, backend API ve PostgreSQL ayrı container’larda çalışır. Servislerin ağ bağlantıları, portları ve çalışma düzeni `docker-compose.yml` üzerinden yönetilir.
