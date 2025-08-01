# Doküman Yönetim Sistemi

Doğal dilde arama, özetleme ve anahtar kelime çıkarımı yapan web uygulaması.

## Özellikler

- **Doküman Yükleme**: PDF, TXT, DOCX formatlarında dosya yükleme
- **Otomatik İşleme**: Yüklenen dokümanların otomatik olarak işlenmesi
- **Metin Çıkarma**: PDF ve DOCX dosyalarından metin çıkarma
- **Özetleme**: Doküman içeriğinin otomatik özetlenmesi
- **Anahtar Kelime Çıkarma**: Dokümanlardan anahtar kelimelerin belirlenmesi
- **Doğal Dilde Arama**: Gelişmiş arama motoru ile doküman arama
- **İlgi Skoru**: Arama sonuçlarının ilgi derecesine göre sıralanması
- **Vurgulama**: Arama terimlerinin doküman içinde vurgulanması

## Teknolojiler

- **Backend**: ASP.NET Core 8.0
- **Veritabanı**: Microsoft SQL Server
- **ORM**: Entity Framework Core
- **Frontend**: Bootstrap 5, Font Awesome
- **Dosya İşleme**: iTextSharp (PDF), DocumentFormat.OpenXml (DOCX)
- **Test**: xUnit, Moq

## Kurulum

### Gereksinimler

- .NET 8.0 SDK
- Microsoft SQL Server (LocalDB veya SQL Server Express)
- Visual Studio 2022 veya Visual Studio Code

### Adımlar

1. Projeyi klonlayın:
```bash
git clone <repository-url>
cd DocumentManagementSystem
```

2. Veritabanı bağlantı dizesini güncelleyin:
`appsettings.json` dosyasında `ConnectionStrings` bölümünü kendi SQL Server kurulumunuza göre düzenleyin.

3. Veritabanını oluşturun:
```bash
dotnet ef database update
```

4. Uygulamayı çalıştırın:
```bash
dotnet run
```

5. Tarayıcıda `https://localhost:5001` adresine gidin.

## Kullanım

### Doküman Yükleme

1. "Doküman Yükle" sayfasına gidin
2. Doküman başlığını girin
3. PDF, TXT veya DOCX dosyasını seçin
4. "Dokümanı Yükle" butonuna tıklayın
5. Sistem otomatik olarak dokümanı işleyecek

### Arama

1. "Arama" sayfasına gidin
2. Arama terimini girin (doğal dilde)
3. İsteğe bağlı filtreler ekleyin:
   - Dosya tipi
   - Tarih aralığı
   - Yükleyen kullanıcı
4. "Ara" butonuna tıklayın
5. Sonuçlar ilgi skoruna göre sıralanacak

### Doküman Yönetimi

- **Görüntüleme**: Doküman detaylarını, özetini ve anahtar kelimelerini görüntüleme
- **Düzenleme**: Doküman başlığını düzenleme
- **Silme**: Dokümanı ve ilgili dosyayı silme
- **Yeniden İşleme**: Başarısız işlemleri yeniden çalıştırma

## API Endpoints

### Doküman İşlemleri

- `GET /Document` - Tüm dokümanları listele
- `GET /Document/Upload` - Doküman yükleme sayfası
- `POST /Document/Upload` - Doküman yükle
- `GET /Document/Details/{id}` - Doküman detayları
- `GET /Document/Edit/{id}` - Doküman düzenleme sayfası
- `POST /Document/Edit/{id}` - Doküman güncelle
- `POST /Document/Delete/{id}` - Doküman sil
- `GET /Document/Search` - Arama sayfası
- `POST /Document/Search` - Arama yap
- `GET /Document/SearchResults` - Arama sonuçları
- `POST /Document/Reprocess/{id}` - Dokümanı yeniden işle

## Test

Unit testleri çalıştırmak için:

```bash
dotnet test
```

### Test Kapsamı

- Doküman yükleme ve erişim testleri
- Özetleme fonksiyonu testleri
- Arama sonucu testleri
- Dosya işleme testleri
- Veritabanı işlemleri testleri

## Proje Yapısı

```
DocumentManagementSystem/
├── Controllers/          # MVC Controllers
├── Data/                # Entity Framework Context
├── Models/              # Entity Models
├── Services/            # Business Logic Services
├── ViewModels/          # View Models
├── Views/               # Razor Views
├── wwwroot/             # Static Files
├── Tests/               # Unit Tests
└── DocumentManagementSystem.Tests/  # Test Project
```

## Güvenlik

- CSRF koruması (AntiForgeryToken)
- Dosya tipi doğrulaması
- Dosya boyutu sınırlaması
- Güvenli dosya adlandırma (GUID)

## Performans

- Asenkron işlemler
- Veritabanı indeksleme
- Dosya önbellekleme
- Sayfalama (pagination)

## Gelecek Geliştirmeler

- [ ] Gemini API entegrasyonu
- [ ] Kullanıcı kimlik doğrulama ve yetkilendirme
- [ ] Doküman kategorileri
- [ ] Gelişmiş arama filtreleri
- [ ] Doküman versiyonlama
- [ ] API dokümantasyonu (Swagger)
- [ ] Docker desteği
- [ ] Cloud deployment

## Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## İletişim

Proje hakkında sorularınız için issue açabilirsiniz. 