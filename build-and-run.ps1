# Docker Build and Run Script for Document Management System
# PowerShell script for Windows

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

# Remove existing images (optional - uncomment if you want to force rebuild)
# Write-Host "🗑️ Removing existing images..." -ForegroundColor Yellow
# docker-compose down --rmi all

# Build and start containers
Write-Host "🔨 Building and starting containers..." -ForegroundColor Yellow
docker-compose up --build -d

# Wait for services to be ready
Write-Host "⏳ Waiting for services to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Check container status
Write-Host "📊 Container Status:" -ForegroundColor Cyan
docker-compose ps

# Show logs
Write-Host "📋 Recent logs:" -ForegroundColor Cyan
docker-compose logs --tail=20

Write-Host "`n🎉 Deployment completed!" -ForegroundColor Green
Write-Host "📱 Application URL: http://localhost:8080" -ForegroundColor Cyan
Write-Host "🗄️ Database URL: localhost:1433" -ForegroundColor Cyan
Write-Host "`n📝 Useful commands:" -ForegroundColor Yellow
Write-Host "  - View logs: docker-compose logs -f" -ForegroundColor White
Write-Host "  - Stop services: docker-compose down" -ForegroundColor White
Write-Host "  - Restart services: docker-compose restart" -ForegroundColor White
Write-Host "  - Access container shell: docker-compose exec webapp /bin/bash" -ForegroundColor White 