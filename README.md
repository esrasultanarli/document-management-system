# Document Management System

Bu proje, ASP.NET Core MVC kullanılarak geliştirilmiş kapsamlı bir belge yönetim sistemidir. Kullanıcıların belge yükleyebilmesi, arama yapabilmesi ve belgeleri yönetebilmesi için tasarlanmıştır.

## 🚀 Özellikler

- **Kullanıcı Yönetimi**: Kayıt olma ve giriş yapma
- **Belge Yükleme**: PDF, DOCX ve diğer belge formatlarını destekler
- **Gelişmiş Arama**: Google Gemini AI entegrasyonu ile akıllı belge arama
- **Belge Yönetimi**: Belge düzenleme, silme ve detay görüntüleme
- **Güvenlik**: Kullanıcı bazlı yetkilendirme sistemi
- **Test Kapsamı**: Kapsamlı unit testler

## 🛠️ Teknolojiler

- **Backend**: ASP.NET Core 8.0 MVC
- **Veritabanı**: Entity Framework Core (SQL Server)
- **AI Entegrasyonu**: Google Gemini API
- **Frontend**: Bootstrap, jQuery, Font Awesome
- **Test**: xUnit

## 📋 Gereksinimler

- .NET 8.0 SDK
- SQL Server (LocalDB veya SQL Server Express)
- Google Gemini API anahtarı

## 🔧 Kurulum

1. **Repository'yi klonlayın**

   ```bash
   git clone https://github.com/esrasultanarli/document-management-system.git
   cd document-management-system
   ```

2. **Bağımlılıkları yükleyin**

   ```bash
   cd DocumentManagementSystem
   dotnet restore
   ```

3. **Veritabanını oluşturun**

   ```bash
   dotnet ef database update
   ```

4. **API anahtarını yapılandırın**

   - `appsettings.Development.json` dosyasını oluşturun ve `GeminiApiKey` değerini ayarlayın
   - Google Cloud Console'dan Gemini API anahtarı alın
   - **ÖNEMLİ**: API anahtarınızı asla GitHub'a yüklemeyin!

5. **Uygulamayı çalıştırın**
   ```bash
   dotnet run
   ```

## 🧪 Testleri Çalıştırma

```bash
cd DocumentManagementSystem.Tests
dotnet test
```

## 📁 Proje Yapısı

```
DocumentManagementSystem/
├── Controllers/          # MVC Controllers
├── Models/              # Entity models
├── Services/            # Business logic services
├── Views/               # Razor views
├── Data/                # Database context
├── ViewModels/          # View models
└── wwwroot/             # Static files

DocumentManagementSystem.Tests/
├── AuthServiceTests.cs
├── DocumentServiceTests.cs
├── GeminiServiceTests.cs
└── ...
```

## 🔐 Güvenlik

- Kullanıcı şifreleri BCrypt ile hashlenir
- JWT token tabanlı kimlik doğrulama
- Kullanıcı bazlı yetkilendirme
- Dosya yükleme güvenliği

## 🔑 API Anahtarı Yapılandırması

### Gemini API Anahtarı Kurulumu

1. **Google Cloud Console'dan API Anahtarı Alın**

   - [Google AI Studio](https://makersuite.google.com/app/apikey) adresine gidin
   - Yeni bir API anahtarı oluşturun

2. **Yerel Geliştirme İçin**

   ```bash
   # appsettings.Development.json dosyasını oluşturun
   cp appsettings.Development.json.example appsettings.Development.json
   ```

   `appsettings.Development.json` dosyasında API anahtarınızı girin:

   ```json
   {
     "GeminiApiKey": "YOUR_ACTUAL_API_KEY_HERE"
   }
   ```

3. **Production Ortamı İçin**

   - Environment variable kullanın: `GEMINI_API_KEY`
   - Docker için: `docker-compose.yml` dosyasında environment variable tanımlayın

4. **Güvenlik Notları**
   - API anahtarınızı asla GitHub'a yüklemeyin
   - `appsettings.Development.json` dosyası `.gitignore` ile korunmaktadır
   - Production ortamında environment variable kullanın

## 🤖 AI Entegrasyonu

Proje, Google Gemini AI kullanarak gelişmiş belge arama özelliği sunar:

- Doğal dil ile belge arama
- Belge içeriği analizi
- Akıllı sonuç sıralama

## 📝 API Endpoints

- `POST /Auth/Login` - Kullanıcı girişi
- `POST /Auth/Register` - Kullanıcı kaydı
- `GET /Document` - Belge listesi
- `POST /Document/Upload` - Belge yükleme
- `GET /Document/Search` - Belge arama
- `GET /Document/{id}` - Belge detayı

## 🚀 Deployment

### Azure'a Deploy Etme

1. Azure App Service oluşturun
2. SQL Database bağlantısını yapılandırın
3. Application Settings'de API anahtarını ayarlayın
4. GitHub Actions ile CI/CD pipeline kurun

### Docker ile Çalıştırma

```bash
docker build -t document-management-system .
docker run -p 8080:80 document-management-system
```

## 🙏 Teşekkürler

- Google Gemini AI
- ASP.NET Core ekibi
- Bootstrap ve diğer açık kaynak projeler
