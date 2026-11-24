# Raf – Film, Oyun ve Müzik Kütüphanesi

Raf, kullanıcıların izledikleri filmleri, oynadıkları oyunları ve dinledikleri albümleri tek yerde
kaydedip puanladığı, incelediği ve listelere eklediği bir medya kütüphanesi portalıdır.
Kullanıcılar birbirini takip eder ve takip ettiklerinin incelemelerini akışta görür.

## Teknolojiler

- ASP.NET Core MVC (.NET 10)
- PostgreSQL + Entity Framework Core (Npgsql)
- ASP.NET Core Identity (Yönetici, Moderatör, Kullanıcı rolleri)
- Swagger (Swashbuckle)
- Bootstrap 5
- Harici API'ler: TMDB (film), RAWG (oyun), Discogs (albüm)

## Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/) (16 veya üstü)
- Git

## Kurulum ve Çalıştırma

### 1. Depoyu klonlayın

```bash
git clone https://github.com/monke-72/raf.git
cd raf
```

### 2. PostgreSQL'i kurun

PostgreSQL'i kurun ve `postgres` kullanıcısının şifresini not alın.
Varsayılan port `5432`'dir.

> **Windows dili Türkçe ise:** Kurulum sihirbazındaki **Locale** seçeneğini `[Default locale]` yerine
> `English, United States` yapın. Aksi hâlde `initdb` "non-ASCII characters" hatası verip veritabanını oluşturamaz.

### 3. Bağlantı dizesini ve API anahtarlarını user-secrets ile ayarlayın

Şifreler ve API anahtarları depoya girmez. `appsettings.json` içindeki değerler bilerek boş bırakılmıştır.
Gerçek değerler `dotnet user-secrets` ile yalnızca sizin bilgisayarınızda saklanır:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=raf;Username=postgres;Password=SIFRENIZ" --project Raf
dotnet user-secrets set "Tmdb:ApiKey" "TMDB_ANAHTARINIZ" --project Raf
dotnet user-secrets set "Rawg:ApiKey" "RAWG_ANAHTARINIZ" --project Raf
dotnet user-secrets set "Discogs:Token" "DISCOGS_TOKENINIZ" --project Raf
```

API anahtarları ücretsiz alınabilir:
- TMDB: https://www.themoviedb.org/settings/api
- RAWG: https://rawg.io/apidocs
- Discogs: https://www.discogs.com/settings/developers ("Generate new token")

Anahtarlar girilmezse uygulama yine çalışır; yalnızca ilgili arama hata mesajı gösterir.

Yönetici hesabı da user-secrets ile tanımlanır. Uygulama ilk açılışta bu bilgilerle bir yönetici hesabı oluşturur:

```bash
dotnet user-secrets set "Yonetici:Email" "yonetici@ornek.com" --project Raf
dotnet user-secrets set "Yonetici:Sifre" "GUCLU_BIR_SIFRE" --project Raf
```

Sitede kayıt olan herkes **Kullanıcı** rolüyle başlar. Roller: Yönetici, Moderatör, Kullanıcı.

### 4. Araçları geri yükleyin ve veritabanını oluşturun

```bash
dotnet tool restore
dotnet ef database update --project Raf
```

> Veritabanı yalnızca migration'lar ile oluşturulur.

### 5. Uygulamayı çalıştırın

```bash
dotnet run --project Raf
```

Konsolda yazan adresi (örn. `http://localhost:5275`) tarayıcıda açın.

## Web API

İncelemeler için tam CRUD destekli bir Web API bulunur. Uygulama geliştirme ortamında çalışırken
`/swagger` adresinden test edilebilir.

| Metot | Adres | Açıklama | Giriş gerekli |
|---|---|---|---|
| GET | `/api/incelemeler?icerikId=` | İncelemeleri listeler (isteğe bağlı içerik filtresi) | Hayır |
| GET | `/api/incelemeler/{id}` | Tek inceleme getirir | Hayır |
| POST | `/api/incelemeler` | Yeni inceleme ekler | Evet |
| PUT | `/api/incelemeler/{id}` | Kendi incelemesini günceller | Evet |
| DELETE | `/api/incelemeler/{id}` | İncelemeyi siler (sahibi, Moderatör veya Yönetici) | Evet |

Giriş gerektiren işlemler için önce sitede giriş yapın; Swagger aynı tarayıcı oturumunu kullanır.

## Proje Yapısı

```
Raf/
├── Controllers/   MVC denetleyicileri
│   └── Api/       Web API denetleyicileri
├── Data/          RafDbContext, seed verileri, migration'lar
├── Models/        Varlık modelleri, DTO'lar ve form modelleri
├── Services/      TMDB, RAWG ve Discogs servisleri
├── Views/         Razor görünümleri
└── wwwroot/       CSS, JS, Bootstrap
```

## Geliştirici

Muhammet Talha Turan – [@monke-72](https://github.com/monke-72)
