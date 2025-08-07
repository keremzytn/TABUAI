# 🚀 TABU.AI Deployment Rehberi

## 📋 Ön Gereksinimler

### Geliştirme Ortamı
- .NET 9 SDK
- Node.js 18+
- PostgreSQL 15+
- Docker & Docker Compose (opsiyonel)

### Üretim Ortamı
- Docker & Docker Compose
- OpenAI API Key
- Domain ve SSL sertifikası (opsiyonel)

## 🔧 Yerel Geliştirme

### 1. Repository'yi Klonlayın
```bash
git clone https://github.com/username/tabu-ai.git
cd tabu-ai
```

### 2. Environment Dosyasını Ayarlayın
```bash
cp .env.example .env
# .env dosyasını düzenleyin ve OpenAI API key'inizi ekleyin
```

### 3. PostgreSQL'i Başlatın
```bash
docker run -d \
  --name tabuai-postgres \
  -e POSTGRES_DB=tabuai_db \
  -e POSTGRES_USER=tabuai_user \
  -e POSTGRES_PASSWORD=tabuai_password \
  -p 5432:5432 \
  postgres:15
```

### 4. Backend'i Çalıştırın
```bash
cd backend
export PATH="$PATH:/home/ubuntu/.dotnet"
dotnet restore
dotnet run --project src/TabuAI.API
```

### 5. Frontend'i Çalıştırın
```bash
cd frontend
npm install
npm start
```

## 🐳 Docker ile Deployment

### 1. Environment Dosyasını Ayarlayın
```bash
cp .env.example .env
# OpenAI API key'inizi ekleyin
```

### 2. Servisleri Başlatın
```bash
docker-compose up -d
```

### 3. Logları Kontrol Edin
```bash
docker-compose logs -f
```

## 🌐 Üretim Deployment'ı

### 1. Docker Compose Üretim Konfigürasyonu

```yaml
# docker-compose.prod.yml
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: ${POSTGRES_DB}
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    restart: always
    networks:
      - tabuai-network

  backend:
    build:
      context: ./backend
      dockerfile: Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}
      - OpenAI__ApiKey=${OPENAI_API_KEY}
    depends_on:
      - postgres
    restart: always
    networks:
      - tabuai-network

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    ports:
      - "80:80"
      - "443:443"
    depends_on:
      - backend
    restart: always
    networks:
      - tabuai-network

  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf
      - ./nginx/ssl:/etc/nginx/ssl
    depends_on:
      - frontend
      - backend
    restart: always
    networks:
      - tabuai-network

volumes:
  postgres_data:

networks:
  tabuai-network:
    driver: bridge
```

### 2. Üretim Deployment'ı
```bash
# Üretim ortamında çalıştırın
docker-compose -f docker-compose.prod.yml up -d
```

## 🔒 Güvenlik Konfigürasyonu

### SSL Sertifikası (Let's Encrypt)
```bash
# Certbot kurulumu
sudo apt install certbot

# SSL sertifikası oluşturma
sudo certbot certonly --standalone -d yourdomain.com

# Sertifikaları nginx'e kopyalama
sudo cp /etc/letsencrypt/live/yourdomain.com/fullchain.pem ./nginx/ssl/
sudo cp /etc/letsencrypt/live/yourdomain.com/privkey.pem ./nginx/ssl/
```

### Database Backup
```bash
# Backup oluşturma
docker exec tabuai-postgres pg_dump -U tabuai_user tabuai_db > backup.sql

# Backup'tan restore etme
docker exec -i tabuai-postgres psql -U tabuai_user tabuai_db < backup.sql
```

## 📊 Monitoring ve Logs

### Application Logs
```bash
# Backend logs
docker-compose logs backend -f

# Frontend logs
docker-compose logs frontend -f

# Database logs
docker-compose logs postgres -f
```

### Health Checks
```bash
# Backend health check
curl http://localhost:5000/api/game/health

# Database health check
docker exec tabuai-postgres pg_isready -U tabuai_user
```

## 🔄 Güncelleme Süreci

### 1. Kod Güncellemesi
```bash
git pull origin main
```

### 2. Container'ları Yeniden Build Etme
```bash
docker-compose build --no-cache
docker-compose up -d
```

### 3. Database Migration (gerekirse)
```bash
docker exec tabuai-backend dotnet ef database update
```

## 🌍 Bulut Deployment'ı

### Azure Container Instances
```bash
# Resource group oluşturma
az group create --name tabuai-rg --location eastus

# Container group deploy etme
az container create \
  --resource-group tabuai-rg \
  --file docker-compose.yml \
  --name tabuai-app
```

### AWS ECS
```bash
# ECS cluster oluşturma
aws ecs create-cluster --cluster-name tabuai-cluster

# Task definition oluşturma
aws ecs register-task-definition --cli-input-json file://task-definition.json
```

### Google Cloud Run
```bash
# Container'ı Cloud Registry'ye push etme
docker tag tabuai-backend gcr.io/PROJECT-ID/tabuai-backend
docker push gcr.io/PROJECT-ID/tabuai-backend

# Cloud Run service deploy etme
gcloud run deploy tabuai-backend \
  --image gcr.io/PROJECT-ID/tabuai-backend \
  --platform managed \
  --region us-central1
```

## 🛠️ Troubleshooting

### Yaygın Sorunlar

1. **Database Bağlantı Sorunu**
   ```bash
   # PostgreSQL çalışıyor mu kontrol edin
   docker ps | grep postgres
   
   # Bağlantı string'ini kontrol edin
   docker logs tabuai-backend | grep "connection"
   ```

2. **OpenAI API Hatası**
   ```bash
   # API key'in doğru set edildiğini kontrol edin
   docker exec tabuai-backend printenv | grep OPENAI
   ```

3. **Frontend Build Hatası**
   ```bash
   # Node modules'ları temizleyin
   rm -rf frontend/node_modules
   npm install
   ```

### Log Analizi
```bash
# Tüm servislerin loglarını takip etme
docker-compose logs -f

# Specific service logları
docker-compose logs backend -f --tail=100
```

### Performance Monitoring
```bash
# Container resource kullanımı
docker stats

# Database performance
docker exec tabuai-postgres psql -U tabuai_user -d tabuai_db -c "SELECT * FROM pg_stat_activity;"
```

## 📞 Destek

Deployment sürecinde sorun yaşıyorsanız:

1. Logları kontrol edin
2. GitHub Issues'da arayın
3. Yeni issue oluşturun
4. Discord topluluğumuzdan yardım alın

---

**Happy Deploying! 🚀**