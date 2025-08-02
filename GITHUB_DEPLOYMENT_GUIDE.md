# GitHub'a Güvenli Deployment Rehberi

## 🔐 API Anahtarı Güvenliği

Projeniz GitHub'a yüklenmeden önce tüm hassas bilgiler güvenli hale getirilmiştir.

### ✅ Yapılan Değişiklikler

1. **appsettings.json** - API anahtarı kaldırıldı
2. **appsettings.Development.json** - Örnek dosya oluşturuldu
3. **docker-compose.yml** - Placeholder kullanıldı
4. **GeminiService.cs** - Hardcoded fallback kaldırıldı
5. **.gitignore** - Hassas dosyalar korundu
6. **Dokümantasyon** - Tüm API anahtarları placeholder ile değiştirildi

### 📁 Güvenlik Dosyaları

- `appsettings.Development.json` - `.gitignore` ile korunuyor
- `appsettings.Production.json` - `.gitignore` ile korunuyor
- `.env` - `.gitignore` ile korunuyor

## 🚀 GitHub'a Yükleme Adımları

### 1. Git Repository Oluşturma

```bash
# Mevcut git durumunu kontrol edin
git status

# Tüm değişiklikleri ekleyin
git add .

# Commit oluşturun
git commit -m "Initial commit: Document Management System with secure API configuration"

# GitHub repository'nizi remote olarak ekleyin
git remote add origin https://github.com/kullaniciadi/repo-adi.git

# Main branch'e push yapın
git push -u origin main
```

### 2. GitHub Repository Ayarları

1. **Repository'yi Private yapın** (önerilen)
2. **README.md** dosyası otomatik görüntülenecek
3. **Issues** ve **Pull Requests** aktif olacak

### 3. Environment Variables (GitHub Secrets)

Production deployment için GitHub Secrets kullanın:

1. Repository Settings > Secrets and variables > Actions
2. Yeni repository secret ekleyin:
   - `GEMINI_API_KEY` = Gerçek API anahtarınız

## 🔧 Geliştirici Kurulumu

### Yerel Geliştirme

```bash
# Repository'yi klonlayın
git clone https://github.com/kullaniciadi/repo-adi.git
cd repo-adi

# API anahtarını yapılandırın
cp appsettings.Development.json.example appsettings.Development.json
# appsettings.Development.json dosyasını düzenleyin ve API anahtarınızı girin

# Bağımlılıkları yükleyin
cd DocumentManagementSystem
dotnet restore

# Veritabanını oluşturun
dotnet ef database update

# Uygulamayı çalıştırın
dotnet run
```

### Docker ile Çalıştırma

```bash
# Environment variable ayarlayın
export GEMINI_API_KEY="your-actual-api-key"

# Docker container'larını başlatın
docker-compose up --build
```

## 📋 Kontrol Listesi

### ✅ Güvenlik Kontrolleri

- [ ] API anahtarı `appsettings.json`'dan kaldırıldı
- [ ] `appsettings.Development.json` `.gitignore`'da
- [ ] `appsettings.Production.json` `.gitignore`'da
- [ ] `.env` dosyası `.gitignore`'da
- [ ] Tüm dokümantasyonda placeholder kullanıldı
- [ ] Hardcoded fallback'ler kaldırıldı

### ✅ GitHub Hazırlığı

- [ ] Repository private yapıldı
- [ ] README.md güncellendi
- [ ] Kurulum talimatları eklendi
- [ ] API anahtarı yapılandırma rehberi eklendi
- [ ] Örnek dosyalar oluşturuldu

### ✅ Dokümantasyon

- [ ] README.md güncellendi
- [ ] API anahtarı kurulum rehberi eklendi
- [ ] Güvenlik notları eklendi
- [ ] Örnek dosyalar oluşturuldu

## 🚨 Önemli Güvenlik Notları

1. **API anahtarınızı asla GitHub'a yüklemeyin**
2. **Repository'yi public yapmayın** (eğer API anahtarı varsa)
3. **Environment variables kullanın** production'da
4. **Düzenli olarak API anahtarınızı değiştirin**
5. **GitHub Secrets kullanın** CI/CD için

## 🔄 Güncelleme Süreci

Gelecekte API anahtarınızı değiştirmeniz gerektiğinde:

1. **Yerel geliştirme**: `appsettings.Development.json` güncelleyin
2. **Production**: Environment variable güncelleyin
3. **GitHub Secrets**: Repository secrets güncelleyin
4. **Docker**: Environment variable güncelleyin

## 📞 Destek

Herhangi bir sorun yaşarsanız:

1. README.md dosyasını kontrol edin
2. GitHub Issues bölümünü kullanın
3. Dokümantasyonu inceleyin

---

**Not**: Bu rehber, projenizi güvenli bir şekilde GitHub'a yüklemeniz için hazırlanmıştır. Tüm hassas bilgiler korunmuş ve güvenlik en iyi uygulamaları uygulanmıştır.
