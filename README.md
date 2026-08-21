TechCart

TechCart, teknoloji ürünlerinin keşfedilmesi ve satın alınması süreçlerini modern bir kullanıcı deneyimiyle sunmayı amaçlayan full-stack bir e-ticaret uygulamasıdır. Proje; katmanlı backend mimarisi, bağımsız frontend uygulaması, otomatik test altyapısı ve container tabanlı geliştirme ortamı ile yapılandırılmıştır.

Proje kapsamı

Kullanıcı kaydı, giriş ve rol tabanlı yetkilendirme

Ürün, kategori, marka ve stok yönetimi

Ürün listeleme, detay görüntüleme, filtreleme ve sıralama

Elasticsearch destekli ürün arama

Sepet ve ürün adedi yönetimi

Ürün karşılaştırma

Sipariş oluşturma, geçmiş görüntüleme ve durum takibi

3D Secure destekli test ödeme entegrasyonu

Satın alınan ürünler için puanlama

Yönetici işlemleri

Teknoloji yığını

Alan

Teknoloji

Frontend

Next.js, React, TypeScript

Backend

ASP.NET Core Web API, .NET 9

Veritabanı

PostgreSQL

Arama

Elasticsearch

Test

xUnit

Container

Docker, Docker Compose

Mimari

Backend, sorumlulukların ayrılmasını sağlayan katmanlı mimari yaklaşımıyla yapılandırılmıştır:

TechCart.API: HTTP istekleri ve API endpoint'leri

TechCart.Application: Uygulama servisleri, DTO'lar ve kullanım senaryoları

TechCart.Domain: Entity'ler, enum'lar ve temel iş kuralları

TechCart.Persistence: PostgreSQL ve veri erişim işlemleri

TechCart.Infrastructure: Elasticsearch, ödeme ve e-posta gibi dış servis entegrasyonları

Kurulum ve çalıştırma

Gereksinimler

Git

Docker Desktop

1. Repository'yi klonlayın

git clone https://github.com/edanur-caglayann/TechCart.git
cd TechCart

2. Docker Desktop'ı başlatın

Docker Desktop açıldıktan sonra Docker Engine'in çalışır durumda olduğundan emin olun.

3. Uygulamayı çalıştırın

docker compose up --build

Docker Compose, uygulama servislerini ayrı container'lar hâlinde aynı ağ üzerinde çalıştırır:

Servis

Adres

Frontend

http://localhost:3000

Backend API

http://localhost:8080

PostgreSQL

localhost:5433

OpenAPI belgesi

http://localhost:8080/openapi/v1.json

Uygulamayı arka planda çalıştırmak için:

docker compose up --build -d

Çalışan servisleri görüntülemek için:

docker compose ps

Uygulamayı durdurmak için:

docker compose down

Docker kullanmadan çalıştırma

Frontend

cd frontend
npm install
npm run dev

Backend API

TechCart ana klasöründe:

dotnet run --project backend/src/TechCart.API

Testler

Tüm backend testlerini çalıştırmak için:

dotnet test backend/TechCart.sln

Yalnızca unit testleri çalıştırmak için:

cd tests/TechCart.UnitTests
dotnet test

Solution dosyası TechCart.slnx biçimindeyse test komutunda TechCart.sln yerine TechCart.slnx kullanılmalıdır.

Container yapısı

TechCart Compose Project
├── techcart-frontend
├── techcart-api
└── techcart-postgres

Frontend ve backend için ayrı Dockerfile kullanılır. PostgreSQL ise resmi PostgreSQL imajı üzerinden çalıştırılır. Servislerin oluşturulması, ağ bağlantıları, portları ve çalışma sırası docker-compose.yml dosyası tarafından yönetilir.
