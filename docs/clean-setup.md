# TechCart — Temiz ortamda backend kurulumu

Bu kurulum, geliştirme verilerini kullanmadan boş bir ortamda
veritabanı yapısını oluşturmayı, örnek ürünleri yüklemeyi ve
API üzerinden listeleme ile arama yapmayı doğrular.

Bu ortamın Compose proje adı techcart-clean'dir.
Canlı demo ve yük testi ortamları ayrıca hazırlanacaktır.

## 1. Gereksinimler

- Docker Desktop çalışır durumda olmalı.
- global.json dosyasının seçtiği .NET SDK kurulu olmalı.
  Mevcut sürüm: 10.0.402; rollForward: latestPatch.
- Terminalde bash, curl, python3 ve openssl kullanılabilmeli.
- Projedeki örnek CSV dosyası mevcut olmalı:
  backend/src/Hosts/TechCart.Seeder/data/electronics_product.csv

Komutlar TechCart ana klasöründen çalıştırılır:

```bash
cd "/Users/cagla/Desktop/TechCart"
```

Başka bir bilgisayarda bu yol, projenin bulunduğu klasöre göre değiştirilir.

## 2. Yerel ayarları hazırlama

İlk kurulumda TechCart ana klasöründe .env.clean dosyası oluşturulur.
Dosya zaten varsa mevcut değerleri korunur.

Dosyanın yapısı:

```dotenv
POSTGRES_DB=techcart_clean
POSTGRES_USER=techcart_clean
POSTGRES_PASSWORD='BURAYA_POSTGRES_PAROLASI'

RABBITMQ_USERNAME=techcart_clean
RABBITMQ_PASSWORD='BURAYA_RABBITMQ_PAROLASI'

JWT_SECRET='BURAYA_JWT_ANAHTARI'

IYZICO_API_KEY='BURAYA_SANDBOX_API_KEY'
IYZICO_SECRET_KEY='BURAYA_SANDBOX_SECRET_KEY'
```

PostgreSQL parolası, RabbitMQ parolası ve JWT anahtarı için
aşağıdaki komut üç ayrı kez çalıştırılır; her çıktı farklı bir
alana yazılır:

```bash
openssl rand -hex 32
```

iyzico alanlarına sandbox hesabının anahtarları yazılır.

.env.clean gerçek ayarları içerir ve Git'e gönderilmez.
Bu belgedeki yer tutucular gerçek değerlerle değiştirilmeden
kuruluma başlanmaz.

Docker Compose dosyası şu komutla doğrulanır:

```bash
docker compose --env-file .env.clean -f deploy/compose.clean.yml -p techcart-clean config --quiet
```

Hatasız tamamlandığında çıktı vermeyebilir.

## 3. Veritabanı tablolarını oluşturma

```bash
bash "/Users/cagla/Desktop/TechCart/scripts/migrate-clean.sh"
```

Script temiz PostgreSQL servisini başlatır, yerel dotnet-ef
aracını geri yükler ve modüllerin mevcut migration'larını sırayla uygular.

Yeni migration dosyası üretmez.
Uygulanmış migration'lar __EFMigrationsHistory üzerinden takip edilir.

Mevcut doğrulanmış sonuç:
- 16 uygulanmış migration.
- Migration geçmişi dahil 13 tablo.

## 4. Örnek ürünleri yükleme

```bash
bash "/Users/cagla/Desktop/TechCart/scripts/seed-clean.sh"
```

Script CSV verisini Seeder üzerinden yükler.
Kategori, marka, ürün, görsel ve stok kayıtları oluşturulur.
Ardından Inventory stokları ürünlerin stok alanına aktarılır.

Mevcut CSV ile doğrulanmış sonuç:
- 11 kategori.
- 919 marka.
- 3731 ürün.
- 3731 ürün görseli.
- 3731 ürün stok kaydı.
- Stok kaydı eksik ürün: 0.
- Ürün ile Inventory arasında stok farkı bulunan ürün: 0.

Stok miktarları ilk yüklemede rastgele üretilir.

## 5. Elasticsearch indeksini doldurma

```bash
bash "/Users/cagla/Desktop/TechCart/scripts/reindex-clean.sh"
```

Script PostgreSQL'deki ürünleri okuyup arama belgelerine dönüştürür.
Temiz Elasticsearch ortamındaki products indeksini yeniden oluşturur
ve bu belgelerle doldurur.

Kaynak veritabanı: localhost:5434 / techcart_clean.
Hedef Elasticsearch: http://localhost:9201.

Bu işlem hedefteki mevcut products indeksini silip yeniden oluşturur.

Belge sayısını doğrulama:

```bash
curl --fail --silent --show-error "http://localhost:9201/products/_count?pretty"
```

Mevcut veri seti için beklenen count: 3731.
_shards.failed değeri: 0.

## 6. API ve Worker'ı başlatma

```bash
docker compose --env-file .env.clean -f deploy/compose.clean.yml -p techcart-clean up -d --build api worker
```

API, Compose ağı üzerinden şu adreslere bağlanır:

- PostgreSQL: postgres:5432.
- RabbitMQ: rabbitmq:5672.
- Elasticsearch: http://elasticsearch:9200.

Bilgisayardan erişim adresleri:

- API: http://localhost:8081.
- Swagger: http://localhost:8081/swagger.
- PostgreSQL: localhost:5434.
- RabbitMQ yönetim paneli: http://localhost:15673.
- Elasticsearch: http://localhost:9201.

## 7. API kontrolleri

PostgreSQL üzerinden ürün listeleme:

```bash
curl --fail --silent --show-error "http://localhost:8081/api/products?page=1&pageSize=1"
```

Bir ürün ve sayfalama bilgileri beklenir.
Mevcut veri setinde toplam ürün sayısı 3731'dir.

Elasticsearch üzerinden arama:

```bash
curl --fail --silent --show-error "http://localhost:8081/api/search/products?q=samsung&page=1&pageSize=1"
```

Mevcut veri setiyle doğrulanan sonuç:
- items içinde 1 ürün.
- pagination.totalItems: 134.

Servis durumlarını kontrol etme:

```bash
docker compose --env-file .env.clean -f deploy/compose.clean.yml -p techcart-clean ps
```

API loglarını inceleme:

```bash
docker compose --env-file .env.clean -f deploy/compose.clean.yml -p techcart-clean logs --since 5m --tail 100 api
```

## 8. Verileri koruyarak durdurma

```bash
docker compose --env-file .env.clean -f deploy/compose.clean.yml -p techcart-clean stop
```

Bu komut container'ları durdurur; volume'larda tutulan verileri korur.

Tekrar başlatma:

```bash
docker compose --env-file .env.clean -f deploy/compose.clean.yml -p techcart-clean up -d api worker
```

## Doğrulamanın kapsamı

Bu rehber PostgreSQL, RabbitMQ, Elasticsearch, Seeder ve API kurulumunu kapsar.
Ürün listeleme ve arama doğrulanmıştır.

Worker temiz PostgreSQL ortaminda calisir. Frontend baglantisi asagidaki
komutlarla secilir. Yeni surumun sandbox odeme donusu ayrica dogrulanmalidir;
canli tahsilat bu kurulumun kapsaminda degildir.

## 9. Frontend ortamini secme ve odeme donus adresleri

Frontend klasorunde ihtiyac duyulan komut calistirilir:

```bash
cd "/Users/cagla/Desktop/TechCart/frontend"
```

| Komut | API hedefi | Backend'i calistirma sekli |
| --- | --- | --- |
| `npm run dev:local` | `http://localhost:5223` | Rider veya dotnet run |
| `npm run dev:docker` | `http://localhost:8080` | Ana docker-compose.yml |
| `npm run dev:clean` | `http://localhost:8081` | deploy/compose.clean.yml |

`npm run dev` de local profilini kullanir. Bu komutlar frontend'i
baslatir; hedef backend'i ayrica baslatmak gerekir.
Frontend adresi http://localhost:3000 olarak kalir. Profil degistirmeden
once mevcut frontend terminalinde Ctrl+C ile durdurulur.

Baslatma script'i NEXT_PUBLIC_API_URL degerini calisan surece verir.
Bu deger Next.js tarafinda .env.local dosyasindan once okunur.
Dosyayi her ortam degisiminde duzenlemek gerekmez.

API icin PublicUrls:ApiBaseUrl, iyzico'ya verilen callback adresinin
kokenini belirler. PublicUrls:FrontendBaseUrl ise callback islendikten
sonra tarayicinin donecegi arayuz adresini belirler.

| Backend kurulumu | PublicUrls:ApiBaseUrl | PublicUrls:FrontendBaseUrl |
| --- | --- | --- |
| Rider / Development JSON | http://localhost:5223 | http://localhost:3000 |
| Gelistirme Compose | http://localhost:8080 | http://localhost:3000 |
| Temiz Compose | http://localhost:8081 | http://localhost:3000 |

API bu iki ayari baslangicta dogrular. Eksik veya gecersiz adresle
baslamaz. Ortami kisinin kimligi degil, istegin ulastigi API kopyasi belirler.
Canli demoda localhost yerine o kurulumun HTTPS adresleri verilecektir.

Docker'daki frontend icin NEXT_PUBLIC_API_URL, imaj olusturulurken build
argumani olarak verilir. Bu deger tarayici koduna derleme sirasinda yazilir;
sadece container'in runtime ortam degiskenini degistirmek yeterli degildir.

Siparisin odeme bekleme suresi 15 dakikadir. Sure degisikliginin
calismasi icin API ve Worker imajlari birlikte yeniden olusturulmalidir.

Callback duzeltmesinden sonra eski iyzico formu tekrar kullanilmaz.
Yeni siparis ve yeni sandbox odeme oturumu olusturulur. Temiz ortamda
beklenen callback adresi http://localhost:8081/api/payments/iyzico/callback
olur; orderId sorgu parametresi API tarafindan eklenir.
Callback token'i POST ile gelir. Tarayici adres cubugundan GET istegi
atmak, callback'i tekrar islemekle ayni sey degildir.

Yeni denemeden sonra Siparislerim sayfasi yenilenir; sonuc yalnizca
URL'deki success parametresinden degil API'nin dondurdugu siparis
ve odeme kayitlarindan dogrulanir.
