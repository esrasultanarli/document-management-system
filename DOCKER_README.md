# Docker Deployment Guide

Bu doküman, Document Management System projesini Docker kullanarak nasıl deploy edeceğinizi açıklar.

## 🐳 Gereksinimler

- Docker Desktop (Windows/Mac/Linux)
- Docker Compose
- En az 4GB RAM
- En az 10GB boş disk alanı

## 🚀 Hızlı Başlangıç

### 1. Projeyi Klonlayın

```bash
git clone <repository-url>
cd AcademyProject
```

### 2. Environment Variables Ayarlayın

```bash
# env.example dosyasını .env olarak kopyalayın
cp env.example .env

# .env dosyasını düzenleyin (gerekirse)
# Özellikle DB_PASSWORD ve GEMINI_API_KEY değerlerini değiştirin
```

### 3. Docker Compose ile Çalıştırın

```bash
# PowerShell kullanarak (Windows)
.\build-and-run.ps1

# Veya manuel olarak
docker-compose up --build -d
```

### 4. Uygulamaya Erişin

- **Web Uygulaması**: http://localhost:8080
- **Veritabanı**: localhost:1433

## 📁 Docker Dosyaları Açıklaması

### Dockerfile

- Multi-stage build kullanır
- .NET 8.0 runtime ve SDK imajlarını kullanır
- Optimized production build sağlar
- Uploads dizini için gerekli izinleri ayarlar

### docker-compose.yml

- **sqlserver**: SQL Server 2022 Express
- **webapp**: .NET Core web uygulaması
- **Volumes**: Veritabanı ve upload dosyaları için kalıcı depolama
- **Networks**: Servisler arası iletişim için özel network

### .dockerignore

- Build context'ini optimize eder
- Gereksiz dosyaları Docker imajına dahil etmez

## 🔧 Konfigürasyon

### Environment Variables

```bash
# Database
DB_PASSWORD=YourStrong@Passw0rd
DB_NAME=DocumentManagementSystem

# Gemini API
GEMINI_API_KEY=your_gemini_api_key

# Application
ASPNETCORE_ENVIRONMENT=Production
```

### Port Mapping

- **8080**: Web uygulaması (HTTP)
- **8443**: Web uygulaması (HTTPS)
- **1433**: SQL Server

## 📊 Monitoring ve Logs

### Container Durumunu Kontrol Edin

```bash
docker-compose ps
```

### Logları Görüntüleyin

```bash
# Tüm servislerin logları
docker-compose logs

# Belirli bir servisin logları
docker-compose logs webapp
docker-compose logs sqlserver

# Canlı log takibi
docker-compose logs -f
```

### Container Shell'ine Erişin

```bash
# Web uygulaması container'ına
docker-compose exec webapp /bin/bash

# SQL Server container'ına
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd
```

## 🛠️ Yaygın İşlemler

### Servisleri Yeniden Başlatın

```bash
docker-compose restart
```

### Servisleri Durdurun

```bash
docker-compose down
```

### Tüm Verileri Silin (Dikkat!)

```bash
docker-compose down -v
```

### Imajları Yeniden Build Edin

```bash
docker-compose build --no-cache
docker-compose up -d
```

## 🔍 Troubleshooting

### SQL Server Bağlantı Sorunu

```bash
# SQL Server'ın hazır olduğunu kontrol edin
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "SELECT 1"
```

### Web Uygulaması Logları

```bash
# Detaylı hata logları için
docker-compose logs webapp --tail=100
```

### Disk Alanı Kontrolü

```bash
# Docker disk kullanımını kontrol edin
docker system df
```

### Temizlik

```bash
# Kullanılmayan Docker kaynaklarını temizleyin
docker system prune -a
```

## 🔒 Güvenlik

### Production Deployment

1. **Güçlü şifreler kullanın**
2. **API anahtarlarını güvenli tutun**
3. **HTTPS kullanın**
4. **Firewall kurallarını yapılandırın**
5. **Regular security updates yapın**

### Environment Variables

- Hassas bilgileri `.env` dosyasında saklayın
- `.env` dosyasını git'e commit etmeyin
- Production'da secrets management kullanın

## 📈 Performance Optimization

### Resource Limits

```yaml
# docker-compose.yml'da servislere resource limitleri ekleyin
services:
  webapp:
    deploy:
      resources:
        limits:
          memory: 1G
          cpus: "0.5"
```

### Database Optimization

- SQL Server memory limitlerini ayarlayın
- Connection pooling kullanın
- Regular maintenance planları oluşturun

## 🚀 Production Deployment

### Docker Swarm veya Kubernetes

- Multi-node deployment için Docker Swarm kullanın
- Load balancing ve high availability için Kubernetes kullanın

### CI/CD Pipeline

- GitHub Actions veya Azure DevOps ile otomatik deployment
- Docker imajlarını container registry'de saklayın
- Automated testing ve security scanning

## 📞 Destek

Sorun yaşarsanız:

1. Logları kontrol edin
2. Docker documentation'ı inceleyin
3. GitHub issues'da arama yapın
4. Geliştirici ekibiyle iletişime geçin
