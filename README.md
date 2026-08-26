<div align="center">

# TechCart

**Modern ve ölçeklenebilir bir full-stack e-ticaret uygulaması**

Next.js • ASP.NET Core • PostgreSQL • RabbitMQ • Docker

</div>

---

## Proje Hakkında

TechCart; teknoloji ürünlerinin keşfedilmesi ve satın alınması süreçlerini kullanıcı dostu bir deneyimle sunan full-stack bir e-ticaret uygulamasıdır.

Backend, event-driven modular monolith mimarisiyle yapılandırılmıştır. Sistemdeki kullanıcı, katalog, stok, sepet, sipariş, ödeme, arama ve puanlama süreçleri bağımsız modüllere ayrılmıştır. Modüller arası asenkron iletişim için RabbitMQ kullanılmaktadır.

## Proje Kapsamı

* Kullanıcı kaydı, giriş ve rol tabanlı yetkilendirme
* Ürün, kategori, marka ve stok yönetimi
* Ürün listeleme ve detay görüntüleme
* Elasticsearch tabanlı arama, filtreleme ve sıralama
* Sepet ve ürün adedi yönetimi
* Stok kontrolü ve stok rezervasyonu
* Sipariş oluşturma, iptal ve durum takibi
* 3D Secure destekli test ödeme entegrasyonu
* Satın alınan ürünler için puanlama
* Yönetici işlemleri

## Mimari Yapı

Backend, tek uygulama olarak çalışan ancak kendi içinde bağımsız iş modüllerine ayrılan modular monolith yapısındadır.

```text
backend/
├── src/
│   ├── BuildingBlocks/
│   ├── Hosts/
│   │   ├── TechCart.Api/
│   │   └── TechCart.Worker/
│   └── Modules/
│       ├── Identity/
│       ├── Catalog/
│       ├── Inventory/
│       ├── Cart/
│       ├── Ordering/
│       ├── Payment/
│       ├── Search/
│       └── Ratings/
├── tests/
└── TechCart.sln
```

```text
TechCart/
├── backend/                         # .NET tabanlı Backend çalışma alanı
│   ├── src/
│   │   ├── BuildingBlocks/          # Ortak altyapı ve çekirdek kütüphaneler (Shared)
│   │   │   ├── TechCart.EventBus         # RabbitMQ / Pub-Sub altyapısı
│   │   │   ├── TechCart.Infrastructure   # Veritabanı, loglama vb. temel ayarlar
│   │   │   └── TechCart.SharedKernel     # Ortak domain objeleri ve exception'lar
│   │   │
│   │   ├── Hosts/                   # Uygulamanın ayağa kalktığı giriş noktaları
│   │   │   ├── TechCart.Api              # API isteklerini karşılayan ana proje
│   │   │   └── TechCart.Worker           # Arka plan işlemlerini yürüten worker
│   │   │
│   │   └── Modules/                 # İzole iş alanları (Domainler)
│   │       ├── Cart
│   │       ├── Catalog
│   │       ├── Identity
│   │       ├── Inventory
│   │       ├── Ordering
│   │       ├── Payment
│   │       ├── Ratings
│   │       └── Search
│   │
│   ├── tests/                       # Test katmanı
│   │   ├── TechCart.ArchitectureTests    # Mimari kuralların testleri
│   │   ├── TechCart.IntegrationTests     # Modüller arası entegrasyon testleri
│   │   └── TechCart.UnitTests            # Birim testleri
│   │
│   └── TechCart.sln                 # .NET Solution dosyası
│
├── deploy/                          # Container (Docker) ve dağıtım yapılandırmaları
│   ├── api.Dockerfile
│   ├── frontend.Dockerfile
│   └── worker.Dockerfile
│
└── frontend/                        # Next.js & React tabanlı kullanıcı arayüzü
    ├── public/
    ├── src/
    ├── next.config.ts
    ├── package.json
    └── (diğer konfigürasyon dosyaları)
 ```   

Her modül kendi içinde şu katmanlara ayrılır:

```text
Domain
Application
Infrastructure
Contracts
```

* **Domain:** Entity’leri ve temel iş kurallarını içerir.
* **Application:** Kullanım senaryolarını ve işlem akışlarını yönetir.
* **Infrastructure:** PostgreSQL, RabbitMQ ve dış servis bağlantılarını uygular.
* **Contracts:** Modüller arası event ve mesaj sözleşmelerini içerir.
* **API:** Frontend’den gelen HTTP isteklerini karşılar.
* **Worker:** RabbitMQ mesajlarını ve arka plan görevlerini işler.

## Teknoloji Yığını

| Alan       | Teknoloji                     |
| ---------- | ----------------------------- |
| Frontend   | Next.js, React, TypeScript    |
| Backend    | ASP.NET Core Web API, .NET 9  |
| Mimari     | Event-Driven Modular Monolith |
| Veritabanı | PostgreSQL                    |
| Mesajlaşma | RabbitMQ                      |
| Arama      | Elasticsearch                 |
| Test       | xUnit                         |
| Container  | Docker, Docker Compose        |

## Kurulum

### Gereksinimler

* Git
* Docker Desktop

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

| Servis                  | Adres                                   |
| ----------------------- | --------------------------------------- |
| Frontend                | `http://localhost:3000`                 |
| Backend API             | `http://localhost:8080`                 |
| OpenAPI                 | `http://localhost:8080/openapi/v1.json` |
| PostgreSQL              | `localhost:5433`                        |
| RabbitMQ                | `localhost:5672`                        |
| RabbitMQ Yönetim Paneli | `http://localhost:15672`                |

RabbitMQ yönetim paneli için varsayılan giriş bilgileri:

```text
Kullanıcı adı: guest
Parola: guest
```

Projeyi arka planda çalıştırmak için:

```bash
docker compose up --build -d
```

Çalışan servisleri görüntülemek için:

```bash
docker compose ps
```

Servis loglarını görüntülemek için:

```bash
docker compose logs
```

Projeyi durdurmak için:

```bash
docker compose down
```

## Docker Kullanmadan Çalıştırma

PostgreSQL ve RabbitMQ servisleri Docker üzerinde bırakılabilir:

```bash
docker compose up -d postgres rabbitmq
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

### Backend API

Repository ana dizininde:

```bash
dotnet run --project backend/src/Hosts/TechCart.Api/TechCart.Api.csproj
```

### Worker

Ayrı bir terminalde:

```bash
dotnet run --project backend/src/Hosts/TechCart.Worker/TechCart.Worker.csproj
```

## Testler

Bütün backend testlerini çalıştırmak için:

```bash
dotnet test backend/TechCart.sln
```

Yalnızca unit testlerini çalıştırmak için:

```bash
dotnet test backend/tests/TechCart.UnitTests/TechCart.UnitTests.csproj
```

Yalnızca integration testlerini çalıştırmak için:

```bash
dotnet test backend/tests/TechCart.IntegrationTests/TechCart.IntegrationTests.csproj
```

Yalnızca architecture testlerini çalıştırmak için:

```bash
dotnet test backend/tests/TechCart.ArchitectureTests/TechCart.ArchitectureTests.csproj
```

Frontend, backend API, Worker, PostgreSQL ve RabbitMQ ayrı container’larda çalışır. Servislerin ağ bağlantıları, portları ve çalışma düzeni `docker-compose.yml` üzerinden yönetilir.
