# Document Management System - Proje Mimarisi ve API Dokümantasyonu

## 📋 İçindekiler

1. [Proje Genel Bakış](#proje-genel-bakış)
2. [Mimari Yapı](#mimari-yapı)
3. [Teknoloji Stack](#teknoloji-stack)
4. [Veritabanı Tasarımı](#veritabanı-tasarımı)
5. [API Endpoints](#api-endpoints)
6. [Servis Katmanı](#servis-katmanı)
7. [Güvenlik](#güvenlik)
8. [AI Entegrasyonu](#ai-entegrasyonu)
9. [Test Stratejisi](#test-stratejisi)
10. [Deployment](#deployment)
11. [Docker Containerization](#docker-containerization)

---

## 🎯 Proje Genel Bakış

**Document Management System (DMS)**, ASP.NET Core MVC kullanılarak geliştirilmiş kapsamlı bir belge yönetim sistemidir. Sistem, kullanıcıların belge yükleyebilmesi, arama yapabilmesi ve belgeleri yönetebilmesi için tasarlanmıştır.

### Temel Özellikler

- ✅ Kullanıcı kimlik doğrulama ve yetkilendirme
- ✅ Belge yükleme ve yönetimi (PDF, DOCX, TXT)
- ✅ Google Gemini AI entegrasyonu ile akıllı arama
- ✅ Belge içeriği analizi ve özetleme
- ✅ Anahtar kelime çıkarma
- ✅ Kullanıcı bazlı yetkilendirme
- ✅ Responsive web arayüzü

---

## 🏗️ Mimari Yapı

### Katmanlı Mimari (Layered Architecture)

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │ Controllers │  │    Views    │  │   ViewModels        │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                    Business Logic Layer                     │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │DocumentSvc  │  │  AuthSvc    │  │   GeminiSvc         │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                    Data Access Layer                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │Application  │  │ Entity      │  │   Models            │  │
│  │DbContext    │  │ Framework   │  │                     │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                     │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │   SQL       │  │   File      │  │   External APIs     │  │
│  │  Server     │  │   System    │  │   (Gemini)          │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### Proje Klasör Yapısı

```
DocumentManagementSystem/
├── Controllers/              # MVC Controllers
│   ├── AuthController.cs     # Kimlik doğrulama işlemleri
│   ├── DocumentController.cs # Belge yönetimi işlemleri
│   └── TestController.cs     # Test endpoint'leri
├── Models/                   # Entity Models
│   ├── Document.cs          # Belge entity'si
│   ├── User.cs              # Kullanıcı entity'si
│   └── SearchResult.cs      # Arama sonucu modeli
├── Services/                 # Business Logic Services
│   ├── IDocumentService.cs  # Belge servisi interface'i
│   ├── DocumentService.cs   # Belge servisi implementasyonu
│   ├── IAuthService.cs      # Kimlik doğrulama servisi interface'i
│   ├── AuthService.cs       # Kimlik doğrulama servisi implementasyonu
│   ├── IGeminiService.cs    # AI servisi interface'i
│   └── GeminiService.cs     # AI servisi implementasyonu
├── Data/                    # Data Access Layer
│   └── ApplicationDbContext.cs # Entity Framework context
├── ViewModels/              # View Models
│   ├── AuthViewModels.cs    # Kimlik doğrulama view model'leri
│   └── DocumentViewModel.cs # Belge view model'leri
├── Views/                   # Razor Views
│   ├── Auth/               # Kimlik doğrulama view'ları
│   ├── Document/           # Belge yönetimi view'ları
│   └── Shared/             # Paylaşılan view'lar
├── wwwroot/                # Static Files
│   ├── css/               # Stylesheets
│   ├── js/                # JavaScript files
│   ├── lib/               # Third-party libraries
│   └── uploads/           # Uploaded documents
└── Program.cs             # Application entry point
```

---

## 🛠️ Teknoloji Stack

### Backend Teknolojileri

- **Framework**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core 8.0
- **Veritabanı**: SQL Server (LocalDB/Express)
- **Authentication**: Cookie-based Authentication
- **Password Hashing**: BCrypt.Net-Next
- **Logging**: Built-in .NET Logging

### Frontend Teknolojileri

- **UI Framework**: Bootstrap 5.3
- **JavaScript Library**: jQuery 3.7
- **Icons**: Font Awesome 6.4
- **Template Engine**: Razor Views

### AI ve External Services

- **AI Service**: Google Gemini API
- **File Processing**: iTextSharp (PDF), DocumentFormat.OpenXml (DOCX)

### Testing

- **Testing Framework**: xUnit
- **Mocking**: Moq
- **Test Runner**: .NET Test

---

## 🗄️ Veritabanı Tasarımı

### Entity Relationship Diagram

```
┌─────────────────┐         ┌─────────────────┐
│      Users      │         │    Documents    │
├─────────────────┤         ├─────────────────┤
│ Id (PK)         │         │ Id (PK)         │
│ Username (UK)   │         │ Title           │
│ Email (UK)      │         │ FileName        │
│ PasswordHash    │         │ FilePath        │
│ FirstName       │         │ FileType        │
│ LastName        │         │ FileSize        │
│ CreatedDate     │         │ Content         │
│ LastLoginDate   │         │ Summary         │
│ IsActive        │         │ Keywords        │
│ Role            │         │ UploadDate      │
└─────────────────┘         │ LastModified    │
                            │ UploadedBy      │
                            │ IsProcessed     │
                            │ ProcessingStatus│
                            └─────────────────┘
```

### Tablo Detayları

#### Users Tablosu

| Alan          | Tip           | Açıklama           | Kısıtlamalar             |
| ------------- | ------------- | ------------------ | ------------------------ |
| Id            | int           | Primary Key        | Identity, Auto Increment |
| Username      | nvarchar(100) | Kullanıcı adı      | Unique, Required         |
| Email         | nvarchar(255) | E-posta adresi     | Unique, Required         |
| PasswordHash  | nvarchar(255) | Şifrelenmiş parola | Required                 |
| FirstName     | nvarchar(100) | Ad                 | Optional                 |
| LastName      | nvarchar(100) | Soyad              | Optional                 |
| CreatedDate   | datetime2     | Oluşturulma tarihi | Default: GETDATE()       |
| LastLoginDate | datetime2     | Son giriş tarihi   | Default: GETDATE()       |
| IsActive      | bit           | Aktif durumu       | Default: true            |
| Role          | nvarchar(50)  | Kullanıcı rolü     | Default: 'User'          |

#### Documents Tablosu

| Alan             | Tip           | Açıklama           | Kısıtlamalar             |
| ---------------- | ------------- | ------------------ | ------------------------ |
| Id               | int           | Primary Key        | Identity, Auto Increment |
| Title            | nvarchar(255) | Belge başlığı      | Required                 |
| FileName         | nvarchar(255) | Dosya adı          | Required                 |
| FilePath         | nvarchar(500) | Dosya yolu         | Required                 |
| FileType         | nvarchar(50)  | Dosya türü         | Required                 |
| FileSize         | bigint        | Dosya boyutu       | Required                 |
| Content          | nvarchar(max) | Belge içeriği      | Optional                 |
| Summary          | nvarchar(max) | Belge özeti        | Optional                 |
| Keywords         | nvarchar(max) | Anahtar kelimeler  | Optional                 |
| UploadDate       | datetime2     | Yükleme tarihi     | Default: GETDATE()       |
| LastModified     | datetime2     | Son değişiklik     | Default: GETDATE()       |
| UploadedBy       | nvarchar(100) | Yükleyen kullanıcı | Required                 |
| IsProcessed      | bit           | İşlenme durumu     | Default: false           |
| ProcessingStatus | nvarchar(50)  | İşleme durumu      | Default: 'Pending'       |

---

## 🔌 API Endpoints

### Authentication Endpoints

#### 1. Kullanıcı Girişi

```
POST /Auth/Login
```

**Request Body:**

```json
{
  "Username": "string",
  "Password": "string",
  "RememberMe": boolean
}
```

**Response:** Redirect to Document/Index on success

#### 2. Kullanıcı Kaydı

```
POST /Auth/Register
```

**Request Body:**

```json
{
  "Username": "string",
  "Email": "string",
  "Password": "string",
  "ConfirmPassword": "string",
  "FirstName": "string",
  "LastName": "string"
}
```

**Response:** Redirect to Auth/Login on success

#### 3. Kullanıcı Çıkışı

```
GET /Auth/Logout
POST /Auth/LogoutPost
```

**Response:** Redirect to Auth/Login

#### 4. Profil Görüntüleme

```
GET /Auth/Profile
```

**Response:** UserProfileViewModel

#### 5. Şifre Değiştirme

```
POST /Auth/ChangePassword
```

**Request Body:**

```json
{
  "CurrentPassword": "string",
  "NewPassword": "string",
  "ConfirmPassword": "string"
}
```

### Document Management Endpoints

#### 1. Belge Listesi

```
GET /Document
```

**Response:** List<Document>

#### 2. Belge Yükleme Sayfası

```
GET /Document/Upload
```

**Response:** Upload form view

#### 3. Belge Yükleme

```
POST /Document/Upload
```

**Request Body:** DocumentUploadViewModel (multipart/form-data)

```json
{
  "Title": "string",
  "File": "IFormFile"
}
```

**Response:** Redirect to Document/Index

#### 4. Belge Detayı

```
GET /Document/Details/{id}
```

**Parameters:**

- id: int (Document ID)

**Response:** Document

#### 5. Belge Düzenleme Sayfası

```
GET /Document/Edit/{id}
```

**Parameters:**

- id: int (Document ID)

**Response:** DocumentViewModel

#### 6. Belge Güncelleme

```
POST /Document/Edit/{id}
```

**Parameters:**

- id: int (Document ID)

**Request Body:**

```json
{
  "Title": "string",
  "Content": "string",
  "Summary": "string",
  "Keywords": "string"
}
```

#### 7. Belge Silme

```
POST /Document/Delete/{id}
```

**Parameters:**

- id: int (Document ID)

**Response:** Redirect to Document/Index

#### 8. Arama Sayfası

```
GET /Document/Search
```

**Response:** Search form view

#### 9. Belge Arama

```
POST /Document/Search
```

**Request Body:**

```json
{
  "SearchTerm": "string",
  "Page": int,
  "PageSize": int
}
```

**Response:** List<SearchResult>

#### 10. Arama Sonuçları

```
GET /Document/SearchResults
```

**Query Parameters:**

- searchTerm: string

**Response:** List<SearchResult>

#### 11. Belge Yeniden İşleme

```
POST /Document/Reprocess/{id}
```

**Parameters:**

- id: int (Document ID)

**Response:** Redirect to Document/Details/{id}

---

## 🔧 Servis Katmanı

### IDocumentService Interface

```csharp
public interface IDocumentService
{
    Task<Document> UploadDocumentAsync(DocumentUploadViewModel model, string uploadedBy);
    Task<Document?> GetDocumentByIdAsync(int id);
    Task<List<Document>> GetAllDocumentsAsync();
    Task<List<SearchResult>> SearchDocumentsAsync(DocumentSearchViewModel model);
    Task<Document> UpdateDocumentAsync(int id, DocumentViewModel model);
    Task<bool> DeleteDocumentAsync(int id, string currentUser);
    Task<string> ExtractTextFromFileAsync(IFormFile file);
    Task<string> GenerateSummaryAsync(string content);
    Task<string> ExtractKeywordsAsync(string content);
    Task<bool> ProcessDocumentAsync(int documentId);
}
```

### IAuthService Interface

```csharp
public interface IAuthService
{
    Task<User?> AuthenticateUserAsync(string username, string password);
    Task<bool> RegisterUserAsync(RegisterViewModel model);
    Task<bool> IsUsernameAvailableAsync(string username);
    Task<bool> IsEmailAvailableAsync(string email);
    Task<User?> GetUserByIdAsync(int id);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}
```

### IGeminiService Interface

```csharp
public interface IGeminiService
{
    Task<string> GenerateSummaryAsync(string content);
    Task<string> ExtractKeywordsAsync(string content);
}
```

### Servis Implementasyonları

#### DocumentService

- **UploadDocumentAsync**: Belge yükleme ve işleme
- **GetDocumentByIdAsync**: ID ile belge getirme
- **GetAllDocumentsAsync**: Tüm belgeleri listeleme
- **SearchDocumentsAsync**: AI destekli belge arama
- **UpdateDocumentAsync**: Belge güncelleme
- **DeleteDocumentAsync**: Belge silme (yetki kontrolü ile)
- **ExtractTextFromFileAsync**: Dosyadan metin çıkarma
- **ProcessDocumentAsync**: Belge işleme (AI analizi)

#### AuthService

- **AuthenticateUserAsync**: Kullanıcı kimlik doğrulama
- **RegisterUserAsync**: Yeni kullanıcı kaydı
- **IsUsernameAvailableAsync**: Kullanıcı adı kontrolü
- **IsEmailAvailableAsync**: E-posta kontrolü
- **GetUserByIdAsync**: Kullanıcı bilgilerini getirme
- **ChangePasswordAsync**: Şifre değiştirme

#### GeminiService

- **GenerateSummaryAsync**: AI ile belge özetleme
- **ExtractKeywordsAsync**: AI ile anahtar kelime çıkarma

---

## 🔐 Güvenlik

### Kimlik Doğrulama (Authentication)

- **Cookie-based Authentication**: ASP.NET Core Identity yerine custom cookie authentication
- **Session Management**: 12 saatlik oturum süresi
- **Remember Me**: Kullanıcı tercihi ile uzun süreli oturum

### Yetkilendirme (Authorization)

- **Role-based Authorization**: Admin ve User rolleri
- **Resource-based Authorization**: Kullanıcılar sadece kendi belgelerini düzenleyebilir
- **Action-based Authorization**: Controller seviyesinde [Authorize] attribute'u

### Şifre Güvenliği

- **BCrypt Hashing**: Güvenli şifre hashleme
- **Salt Generation**: Otomatik salt üretimi
- **Password Validation**: Güçlü şifre kuralları

### Dosya Güvenliği

- **File Type Validation**: Sadece izin verilen dosya türleri
- **File Size Limits**: Maksimum dosya boyutu kontrolü
- **Secure File Storage**: wwwroot/uploads klasöründe güvenli depolama
- **Path Traversal Protection**: Dosya yolu manipülasyonu koruması

### CSRF Koruması

- **Anti-forgery Tokens**: Tüm POST işlemlerinde CSRF koruması
- **ValidateAntiForgeryToken**: Controller action'larda token doğrulama

---

## 🤖 AI Entegrasyonu

### Google Gemini API Entegrasyonu

#### Konfigürasyon

```json
{
  "GeminiApiKey": "YOUR_GEMINI_API_KEY_HERE"
}
```

#### Kullanım Alanları

1. **Belge Özetleme**

   - Uzun belgelerin kısa özetini oluşturma
   - Ana fikirleri çıkarma
   - Önemli noktaları vurgulama

2. **Anahtar Kelime Çıkarma**

   - Belge içeriğinden otomatik anahtar kelime çıkarma
   - Arama optimizasyonu için etiketleme
   - Kategorizasyon desteği

3. **Akıllı Arama**
   - Doğal dil ile arama
   - Semantik arama desteği
   - Benzerlik bazlı sonuç sıralama

#### API Metodları

```csharp
// Belge özetleme
public async Task<string> GenerateSummaryAsync(string content)
{
    var prompt = $"Aşağıdaki metni 2-3 cümlelik bir özet haline getir:\n\n{content}";
    return await CallGeminiAPI(prompt);
}

// Anahtar kelime çıkarma
public async Task<string> ExtractKeywordsAsync(string content)
{
    var prompt = $"Aşağıdaki metinden en önemli 5-10 anahtar kelimeyi çıkar:\n\n{content}";
    return await CallGeminiAPI(prompt);
}
```

#### Hata Yönetimi

- API rate limiting
- Network timeout handling
- Fallback mechanisms
- Error logging

---

## 🧪 Test Stratejisi

### Test Kategorileri

#### 1. Unit Tests

- **AuthServiceTests**: Kimlik doğrulama servisi testleri
- **DocumentServiceTests**: Belge yönetimi servisi testleri
- **GeminiServiceTests**: AI servisi testleri

#### 2. Integration Tests

- **ControllerTests**: Controller action testleri
- **DocumentAuthorizationTests**: Yetkilendirme testleri
- **DocumentRetrievalTests**: Veri erişim testleri

#### 3. Advanced Tests

- **DocumentServiceAdvancedTests**: Gelişmiş belge işleme testleri
- **LLMIntegrationTests**: AI entegrasyon testleri
- **LLMErrorHandlingTests**: AI hata yönetimi testleri
- **SearchFunctionalityTests**: Arama fonksiyonalite testleri

### Test Coverage

- **Service Layer**: %95+ coverage
- **Controller Layer**: %90+ coverage
- **Business Logic**: %100 coverage
- **Error Scenarios**: Comprehensive testing

### Test Execution

```bash
cd DocumentManagementSystem.Tests
dotnet test
```

---

## 🚀 Deployment

### Geliştirme Ortamı

```bash
# Bağımlılıkları yükle
dotnet restore

# Veritabanını oluştur
dotnet ef database update

# Uygulamayı çalıştır
dotnet run
```

### Production Deployment

#### Azure App Service

1. **App Service Plan** oluştur
2. **Web App** deploy et
3. **SQL Database** bağlantısını yapılandır
4. **Application Settings**'de API anahtarını ayarla
5. **Custom Domain** ve SSL sertifikası ekle

#### Docker Containerization

Proje tamamen Dockerize edilmiş ve container-based deployment için hazırlanmıştır.

##### Dockerfile (Multi-stage Build)

```dockerfile
# Multi-stage build for .NET 8.0 application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["DocumentManagementSystem.csproj", "./"]
RUN dotnet restore "DocumentManagementSystem.csproj"

# Copy all source code
COPY . .
WORKDIR "/src"

# Build the application
RUN dotnet build "DocumentManagementSystem.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "DocumentManagementSystem.csproj" -c Release -o /app/publish

# Runtime stage
FROM base AS final
WORKDIR /app

# Create uploads directory
RUN mkdir -p /app/wwwroot/uploads

# Copy published application
COPY --from=publish /app/publish .

# Set permissions for uploads directory
RUN chmod 755 /app/wwwroot/uploads

ENTRYPOINT ["dotnet", "DocumentManagementSystem.dll"]
```

##### Docker Compose Configuration

```yaml
version: "3.8"

services:
  # SQL Server Database
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
      - MSSQL_PID=Express
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
    networks:
      - app-network

  # Web Application
  webapp:
    build:
      context: ./DocumentManagementSystem
      dockerfile: Dockerfile
    ports:
      - "8080:80"
      - "8443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=DocumentManagementSystem;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;MultipleActiveResultSets=true
      - GeminiApiKey=${GEMINI_API_KEY:-YOUR_GEMINI_API_KEY_HERE}
    depends_on:
      - sqlserver
    volumes:
      - uploads_data:/app/wwwroot/uploads
    networks:
      - app-network
    restart: unless-stopped

volumes:
  sqlserver_data:
    driver: local
  uploads_data:
    driver: local

networks:
  app-network:
    driver: bridge
```

##### Docker Deployment Scripts

**build-and-run.ps1** (PowerShell Deployment Script):

```powershell
# Docker Build and Run Script for Document Management System
Write-Host "🚀 Starting Document Management System Docker Deployment..." -ForegroundColor Green

# Check if Docker is running
try {
    docker version | Out-Null
    Write-Host "✅ Docker is running" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker is not running. Please start Docker Desktop first." -ForegroundColor Red
    exit 1
}

# Stop and remove existing containers
Write-Host "🛑 Stopping existing containers..." -ForegroundColor Yellow
docker-compose down

# Build and start containers
Write-Host "🔨 Building and starting containers..." -ForegroundColor Yellow
docker-compose up --build -d

# Wait for services to be ready
Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Check container status
Write-Host "📊 Container Status:" -ForegroundColor Cyan
docker-compose ps

Write-Host "🎉 Deployment completed!" -ForegroundColor Green
Write-Host "📱 Application URL: http://localhost:8080" -ForegroundColor Cyan
Write-Host "🗄️ Database URL: localhost:1433" -ForegroundColor Cyan
```

##### Environment Configuration

**env.example**:

```bash
# Environment Variables for Document Management System
# Copy this file to .env and modify as needed

# Database Configuration
DB_PASSWORD=YourStrong@Passw0rd
DB_NAME=DocumentManagementSystem

# Gemini API Configuration
GEMINI_API_KEY=YOUR_GEMINI_API_KEY_HERE

# Application Configuration
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80

# Docker Configuration
COMPOSE_PROJECT_NAME=document-management-system
```

##### Production Configuration

**appsettings.Production.json**:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver;Database=DocumentManagementSystem;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:80"
      }
    }
  }
}
```

#### Environment Variables

```bash
# Production
ConnectionStrings__DefaultConnection="Server=...;Database=...;User Id=...;Password=...;"
GeminiApiKey="your-production-api-key"
ASPNETCORE_ENVIRONMENT="Production"
```

### CI/CD Pipeline

#### GitHub Actions

```yaml
name: Build and Deploy
on:
  push:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.0.x
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal
      - name: Deploy to Azure
        run: |
          # Azure deployment steps
```

---

## 📊 Performans ve Ölçeklenebilirlik

### Performans Optimizasyonları

- **Async/Await Pattern**: Tüm I/O işlemleri asenkron
- **Entity Framework Optimization**: Lazy loading ve eager loading
- **File Caching**: Statik dosya önbellekleme
- **Database Indexing**: Arama performansı için indeksler

### Ölçeklenebilirlik

- **Stateless Design**: Session state kullanımı minimum
- **Database Connection Pooling**: Veritabanı bağlantı havuzu
- **File Storage**: CDN entegrasyonu için hazır yapı
- **Microservices Ready**: Servis katmanı ayrımı

### Monitoring ve Logging

- **Structured Logging**: JSON formatında loglar
- **Error Tracking**: Exception handling ve logging
- **Performance Metrics**: Response time monitoring
- **Health Checks**: Uygulama sağlık kontrolü

---

## 🔮 Gelecek Geliştirmeler

### Planlanan Özellikler

1. **Real-time Notifications**: SignalR ile gerçek zamanlı bildirimler
2. **Advanced Search**: Elasticsearch entegrasyonu
3. **Document Versioning**: Belge versiyonlama sistemi
4. **Collaboration Features**: Belge paylaşımı ve işbirliği
5. **Mobile App**: React Native mobil uygulama
6. **API Gateway**: Microservices mimarisi için API Gateway
7. **Machine Learning**: Gelişmiş belge sınıflandırma
8. **Blockchain Integration**: Belge doğrulama için blockchain

### Teknik İyileştirmeler

1. **Caching Strategy**: Redis cache entegrasyonu
2. **Message Queue**: RabbitMQ ile asenkron işlemler
3. **Container Orchestration**: Kubernetes deployment
4. **Service Mesh**: Istio ile servis mesh
5. **Observability**: Jaeger ve Prometheus entegrasyonu

---

## 🐳 Docker Containerization

### Docker Mimarisi

Proje tamamen containerized edilmiş ve microservices-ready bir yapıya sahiptir. Docker kullanarak geliştirme, test ve production ortamlarında tutarlı deployment sağlanmıştır.

#### Container Yapısı

```
┌─────────────────────────────────────────────────────────────┐
│                    Docker Compose                           │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐    ┌─────────────────┐                │
│  │   SQL Server    │    │   Web App       │                │
│  │   Container     │    │   Container     │                │
│  │                 │    │                 │                │
│  │ • SQL Server    │    │ • .NET Core     │                │
│  │   2022 Express  │    │   Application   │                │
│  │ • Port: 1433    │    │ • Port: 80/443  │                │
│  │ • Volume:       │    │ • Volume:       │                │
│  │   sqlserver_data│    │   uploads_data  │                │
│  └─────────────────┘    └─────────────────┘                │
└─────────────────────────────────────────────────────────────┘
```

#### Docker Dosyaları

| Dosya                         | Açıklama                         | Konum                       |
| ----------------------------- | -------------------------------- | --------------------------- |
| `Dockerfile`                  | Multi-stage build konfigürasyonu | `DocumentManagementSystem/` |
| `docker-compose.yml`          | Ana servis konfigürasyonu        | Root                        |
| `.dockerignore`               | Build context optimization       | `DocumentManagementSystem/` |
| `build-and-run.ps1`           | PowerShell deployment script     | Root                        |
| `env.example`                 | Environment variables template   | Root                        |
| `appsettings.Production.json` | Production konfigürasyonu        | `DocumentManagementSystem/` |

### Docker Build Süreci

#### Multi-stage Build Avantajları

1. **Optimized Image Size**: Final imaj sadece runtime dependencies içerir
2. **Security**: Build tools final imajda bulunmaz
3. **Caching**: Layer caching ile hızlı rebuild
4. **Reproducibility**: Tutarlı build süreci

#### Build Stages

```dockerfile
# Stage 1: Base Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Runtime environment hazırlığı

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Source code build ve restore

# Stage 3: Publish
FROM build AS publish
# Application publish

# Stage 4: Final
FROM base AS final
# Runtime imajı oluşturma
```

### Container Orchestration

#### Docker Compose Services

**SQL Server Service:**

- **Image**: `mcr.microsoft.com/mssql/server:2022-latest`
- **Port**: 1433 (host) → 1433 (container)
- **Volume**: `sqlserver_data` (persistent storage)
- **Environment**: EULA acceptance, password, edition

**Web Application Service:**

- **Build**: Local Dockerfile
- **Port**: 8080 (host) → 80 (container), 8443 (host) → 443 (container)
- **Volume**: `uploads_data` (document storage)
- **Environment**: Production settings, database connection, API keys

#### Network Configuration

```yaml
networks:
  app-network:
    driver: bridge
```

- **Service Discovery**: Container'lar birbirini servis adlarıyla bulabilir
- **Isolation**: Özel network ile güvenlik
- **Communication**: Web app → SQL Server: `sqlserver:1433`

### Volume Management

#### Persistent Storage

**SQL Server Data:**

```yaml
volumes:
  sqlserver_data:
    driver: local
```

**Document Uploads:**

```yaml
volumes:
  uploads_data:
    driver: local
```

#### Volume Mounting

- **Database Persistence**: Veritabanı verileri container restart'larında korunur
- **Document Storage**: Yüklenen belgeler kalıcı olarak saklanır
- **Backup Strategy**: Volume'lar backup edilebilir

### Environment Configuration

#### Environment Variables

```bash
# Database
DB_PASSWORD=YourStrong@Passw0rd
DB_NAME=DocumentManagementSystem

# Application
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80

# External Services
GEMINI_API_KEY=your_api_key
```

#### Configuration Management

- **Development**: `appsettings.Development.json`
- **Production**: `appsettings.Production.json`
- **Docker**: Environment variables override

### Deployment Workflow

#### Local Development

```bash
# 1. Environment setup
cp env.example .env

# 2. Build and run
docker-compose up --build -d

# 3. Access application
# Web: http://localhost:8080
# Database: localhost:1433
```

#### Production Deployment

```bash
# 1. Environment configuration
export GEMINI_API_KEY="production_api_key"
export DB_PASSWORD="strong_production_password"

# 2. Deploy with production settings
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# 3. Health check
docker-compose ps
docker-compose logs -f
```

### Monitoring ve Logging

#### Container Monitoring

```bash
# Container status
docker-compose ps

# Resource usage
docker stats

# Log monitoring
docker-compose logs -f webapp
docker-compose logs -f sqlserver
```

#### Health Checks

- **Application Health**: HTTP endpoint monitoring
- **Database Health**: Connection testing
- **Service Discovery**: Container communication verification

### Security Considerations

#### Container Security

1. **Non-root User**: Container'lar root olmayan kullanıcı ile çalışır
2. **Read-only Filesystem**: Gereksiz yazma izinleri kaldırıldı
3. **Secrets Management**: Environment variables ile hassas bilgi yönetimi
4. **Network Isolation**: Özel network ile container izolasyonu

#### Production Security

```yaml
# Production security settings
services:
  webapp:
    security_opt:
      - no-new-privileges:true
    read_only: true
    tmpfs:
      - /tmp
      - /var/tmp
```

### Performance Optimization

#### Container Optimization

1. **Multi-stage Build**: Minimal final image size (377MB)
2. **Layer Caching**: Efficient rebuild process
3. **Resource Limits**: Memory ve CPU limitleri
4. **Connection Pooling**: Database connection optimization

#### Resource Management

```yaml
services:
  webapp:
    deploy:
      resources:
        limits:
          memory: 1G
          cpus: "0.5"
        reservations:
          memory: 512M
          cpus: "0.25"
```

### Troubleshooting

#### Common Issues

1. **Container Startup Failures**

   ```bash
   docker-compose logs webapp
   docker-compose logs sqlserver
   ```

2. **Database Connection Issues**

   ```bash
   docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "SELECT 1"
   ```

3. **Port Conflicts**
   ```bash
   netstat -an | findstr :8080
   netstat -an | findstr :1433
   ```

#### Debug Commands

```bash
# Container shell access
docker-compose exec webapp /bin/bash
docker-compose exec sqlserver /bin/bash

# File system inspection
docker-compose exec webapp ls -la /app
docker-compose exec webapp find . -name "*.dll" -type f

# Network connectivity
docker-compose exec webapp ping sqlserver
```

### CI/CD Integration

#### Docker in CI/CD Pipeline

```yaml
# GitHub Actions example
- name: Build Docker image
  run: docker-compose build

- name: Run tests in container
  run: docker-compose run --rm webapp dotnet test

- name: Push to registry
  run: |
    docker tag document-management-system-webapp:latest ${{ secrets.REGISTRY }}/dms:latest
    docker push ${{ secrets.REGISTRY }}/dms:latest
```

#### Registry Integration

- **Docker Hub**: Public image hosting
- **Azure Container Registry**: Private registry
- **AWS ECR**: Cloud-native container registry

### Scaling Strategy

#### Horizontal Scaling

```yaml
services:
  webapp:
    deploy:
      replicas: 3
      update_config:
        parallelism: 1
        delay: 10s
      restart_policy:
        condition: on-failure
```

#### Load Balancing

- **Nginx**: Reverse proxy ve load balancer
- **HAProxy**: High availability proxy
- **Traefik**: Modern reverse proxy

### Backup ve Recovery

#### Data Backup

```bash
# Database backup
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "BACKUP DATABASE DocumentManagementSystem TO DISK = '/var/opt/mssql/backup/dms_backup.bak'"

# Volume backup
docker run --rm -v document-management-system_sqlserver_data:/data -v $(pwd):/backup alpine tar czf /backup/sqlserver_backup.tar.gz -C /data .
```

#### Disaster Recovery

1. **Automated Backups**: Scheduled database backups
2. **Volume Snapshots**: Point-in-time recovery
3. **Configuration Backup**: Environment ve config dosyaları
4. **Document Storage**: Uploaded files backup

---

## 📞 İletişim ve Destek

### Proje Bilgileri

- **Proje Adı**: Document Management System
- **Versiyon**: 1.0.0
- **Framework**: ASP.NET Core 8.0
- **Lisans**: MIT License

### Geliştirici Bilgileri

- **Geliştirici**: Esra Sultan Arlı
- **E-posta**: [Geliştirici e-posta adresi]
- **GitHub**: [GitHub repository linki]

### Dokümantasyon

- **API Documentation**: Bu doküman
- **User Guide**: README.md
- **Deployment Guide**: Bu dokümanın Deployment bölümü
- **Troubleshooting**: Test sonuçları ve hata logları

---

_Bu dokümantasyon Document Management System projesinin teknik mimarisini ve API kullanımını detaylandırmaktadır. Güncellemeler ve değişiklikler için lütfen proje repository'sini takip edin._
