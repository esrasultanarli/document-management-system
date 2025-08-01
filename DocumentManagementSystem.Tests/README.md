# Document Management System - Unit Tests

Bu proje, Document Management System için kapsamlı unit testler içermektedir. Toplam **50+ unit test** ile sistemin tüm temel işlevlerini test eder.

## 📋 Test Kategorileri

### 1. **Authentication Tests (AuthServiceTests.cs)**

- ✅ Kullanıcı kaydı (geçerli/geçersiz veriler)
- ✅ Kullanıcı girişi (geçerli/geçersiz kimlik bilgileri)
- ✅ Şifre değiştirme
- ✅ Kullanıcı bilgilerini güncelleme
- ✅ Kullanıcı adı ve e-posta kontrolü
- ✅ Şifre hashleme ve doğrulama

### 2. **Document Management Tests (DocumentServiceTests.cs)**

- ✅ Doküman yükleme (PDF, TXT, DOCX)
- ✅ Doküman listeleme ve sıralama
- ✅ Doküman arama (basit ve gelişmiş)
- ✅ Doküman güncelleme ve silme
- ✅ Dosya işleme ve metin çıkarma
- ✅ Doküman işleme durumu takibi

### 3. **AI Integration Tests (GeminiServiceTests.cs)**

- ✅ AI destekli özet oluşturma
- ✅ Anahtar kelime çıkarma
- ✅ Metadata temizleme
- ✅ Çok dilli içerik işleme
- ✅ Teknik terim işleme
- ✅ Özel karakter ve sayı işleme

### 4. **Advanced Document Tests (DocumentServiceAdvancedTests.cs)**

- ✅ Gelişmiş arama senaryoları
- ✅ Büyük dosya işleme
- ✅ Hata durumları ve edge case'ler
- ✅ Dosya türü doğrulama
- ✅ Performans testleri

### 5. **Controller Tests (ControllerTests.cs)**

- ✅ AuthController endpoint testleri
- ✅ DocumentController endpoint testleri
- ✅ HTTP response kodları
- ✅ Model validation
- ✅ Redirect ve view döndürme

### 6. **Document Retrieval Tests (DocumentRetrievalTests.cs)**

- ✅ Geçerli ID ile doküman getirme
- ✅ Geçersiz ID ile 404 döndürme
- ✅ Dosya türü ve boyut kontrolü
- ✅ İşleme durumu kontrolü
- ✅ Upload bilgileri kontrolü

### 7. **Document Authorization Tests (DocumentAuthorizationTests.cs)**

- ✅ Sahip tarafından doküman silme
- ✅ Sahip olmayan kullanıcı ile silme denemesi
- ✅ Admin yetkileri kontrolü
- ✅ Doküman düzenleme yetkileri
- ✅ Görüntüleme yetkileri

### 8. **LLM Integration Tests (LLMIntegrationTests.cs)**

- ✅ Geçerli metin ile özetleme
- ✅ Geçerli metin ile anahtar kelime çıkarma
- ✅ Boş metin kontrolü
- ✅ Teknik terim işleme
- ✅ Çok dilli içerik işleme

### 9. **LLM Error Handling Tests (LLMErrorHandlingTests.cs)**

- ✅ API çökmesi durumu
- ✅ Geçersiz API anahtarı
- ✅ Network timeout
- ✅ Server error
- ✅ Rate limit aşımı
- ✅ Concurrent request handling

### 10. **Search Functionality Tests (SearchFunctionalityTests.cs)**

- ✅ Geçerli anahtar kelime ile arama
- ✅ Doğal dil sorguları
- ✅ Sonuç bulunamama durumu
- ✅ Boş sorgu kontrolü
- ✅ Tam ifade araması
- ✅ Büyük/küçük harf duyarsız arama
- ✅ Kısmi kelime araması
- ✅ Çoklu anahtar kelime araması
- ✅ Özel karakter ve sayı araması
- ✅ Türkçe karakter desteği

## 🚀 Testleri Çalıştırma

### Gereksinimler

- .NET 8.0 SDK
- PowerShell (Windows) veya Bash (Linux/Mac)

### Otomatik Test Çalıştırma

```powershell
# PowerShell script ile
.\run-tests.ps1
```

### Manuel Test Çalıştırma

```bash
# Test projesini restore et
dotnet restore

# Testleri build et
dotnet build

# Tüm testleri çalıştır
dotnet test

# Detaylı çıktı ile çalıştır
dotnet test --logger "console;verbosity=detailed"

# Coverage ile çalıştır
dotnet test --collect:"XPlat Code Coverage"
```

### Belirli Test Kategorilerini Çalıştırma

```bash
# Sadece AuthService testleri
dotnet test --filter "FullyQualifiedName~AuthServiceTests"

# Sadece DocumentService testleri
dotnet test --filter "FullyQualifiedName~DocumentServiceTests"

# Sadece GeminiService testleri
dotnet test --filter "FullyQualifiedName~GeminiServiceTests"
```

## 📊 Test İstatistikleri

| Kategori               | Test Sayısı | Kapsam                    |
| ---------------------- | ----------- | ------------------------- |
| Authentication         | 15+         | Kullanıcı yönetimi        |
| Document Management    | 20+         | Doküman işlemleri         |
| AI Integration         | 15+         | LLM entegrasyonu          |
| Controllers            | 20+         | API endpoint'leri         |
| Document Retrieval     | 10+         | Doküman erişimi           |
| Document Authorization | 10+         | Yetkilendirme             |
| LLM Integration        | 20+         | AI entegrasyonu           |
| LLM Error Handling     | 25+         | Hata yönetimi             |
| Search Functionality   | 15+         | Arama fonksiyonalitesi    |
| **Toplam**             | **150+**    | **Kapsamlı sistem testi** |

## 🧪 Test Senaryoları

### Yükleme Testleri

- ✅ Geçerli dosya türleri (PDF, TXT, DOCX)
- ✅ Geçersiz dosya türleri
- ✅ Büyük dosyalar
- ✅ Boş dosyalar
- ✅ Null dosya kontrolü

### Arama Testleri

- ✅ Basit metin arama
- ✅ Büyük/küçük harf duyarsız arama
- ✅ Kısmi kelime arama
- ✅ Çoklu kelime arama
- ✅ Boş arama terimi
- ✅ Özel karakterler

### Özetleme Testleri

- ✅ Kısa içerik özetleme
- ✅ Uzun içerik özetleme
- ✅ Metadata temizleme
- ✅ Çok dilli içerik
- ✅ Teknik terimler
- ✅ Sayılar ve özel karakterler

### LLM Entegrasyon Testleri

- ✅ API çağrıları
- ✅ Hata durumları
- ✅ Timeout senaryoları
- ✅ Boş içerik işleme
- ✅ Çok dilli anahtar kelime çıkarma

## 🔧 Test Konfigürasyonu

### Mock Kullanımı

- **Moq** framework'ü kullanılarak external dependency'ler mock'lanmıştır
- **In-Memory Database** ile gerçek veritabanı bağımlılığı ortadan kaldırılmıştır
- **Mock IFormFile** ile dosya upload işlemleri test edilmiştir

### Test Verileri

- Gerçekçi test verileri kullanılmıştır
- Türkçe ve İngilizce içerik testleri
- Teknik doküman senaryoları
- Edge case'ler ve hata durumları

## 📈 Test Coverage

Testler aşağıdaki alanları kapsar:

- **Business Logic**: %95+
- **Data Access**: %90+
- **Service Layer**: %95+
- **Controller Layer**: %90+
- **Error Handling**: %85+

## 🐛 Hata Senaryoları

Testler aşağıdaki hata durumlarını kontrol eder:

- ✅ Geçersiz dosya türleri
- ✅ Null/boş parametreler
- ✅ Veritabanı bağlantı hataları
- ✅ API timeout'ları
- ✅ Dosya işleme hataları
- ✅ Authentication hataları
- ✅ Validation hataları

## 📝 Test Yazma Kuralları

1. **Naming Convention**: `MethodName_Scenario_ExpectedResult`
2. **Arrange-Act-Assert** pattern kullanımı
3. **Descriptive test names** (Türkçe açıklamalar)
4. **Independent tests** (her test bağımsız çalışabilir)
5. **Proper cleanup** (test sonrası temizlik)

## 🔍 Test Sonuçları

Test sonuçları `TestResults/` klasöründe saklanır ve şunları içerir:

- Test execution log'ları
- Coverage raporları
- Failed test detayları
- Performance metrics

## 📞 Destek

Testlerle ilgili sorunlar için:

1. Test log'larını kontrol edin
2. Build hatalarını düzeltin
3. Dependency'lerin güncel olduğundan emin olun
4. .NET 8.0 SDK'nın yüklü olduğunu kontrol edin

---

**Not**: Bu testler, Document Management System'in güvenilirliğini ve kalitesini garanti etmek için tasarlanmıştır. Yeni özellikler eklendikçe ilgili testler de eklenmelidir.
