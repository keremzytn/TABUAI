# 🎯 TABU.AI - Prompt Engineering Oyunu

Yapay zeka ile etkili iletişim kurma becerilerinizi geliştirin! Klasik TABU oyunundan ilham alan bu eğlenceli deneyimle, AI'ya nasıl daha iyi talimatlar vereceğinizi öğrenin.

## 📋 İçindekiler

- [Özellikler](#-özellikler)
- [Teknolojiler](#-teknolojiler)
- [Kurulum](#-kurulum)
- [Kullanım](#-kullanım)
- [Konfigürasyon](#-konfigürasyon)
- [Proje Yapısı](#-proje-yapısı)

## ✨ Özellikler

- 🤖 **Groq AI Entegrasyonu**: Gerçek zamanlı AI değerlendirmesi
- 🎮 **İki Oyun Modu**: Demo ve AI modu
- 📊 **Prompt Kalite Analizi**: AI tarafından prompt kalitesi değerlendirmesi
- 🏆 **Liderlik Tablosu**: En iyi oyuncuları takip edin
- 💡 **İyileştirme Önerileri**: AI'dan prompt yazma ipuçları
- 📱 **Responsive Tasarım**: Mobil ve masaüstü uyumlu
- 🎨 **Modern UI**: Minimal ve kullanıcı dostu arayüz

## 🛠 Teknolojiler

### Frontend
- **Angular 19** - Modern web framework
- **TypeScript** - Type-safe JavaScript
- **SCSS** - Gelişmiş CSS
- **RxJS** - Reactive programming

### Backend
- **.NET 9.0** - Modern backend framework
- **Entity Framework Core** - ORM
- **PostgreSQL** - Veritabanı
- **MediatR** - CQRS pattern
- **AutoMapper** - Object mapping

### AI
- **Groq API** - Hızlı AI inference
- **Llama 3.1** - Güçlü dil modeli

## 🚀 Kurulum

### Gereksinimler

- Node.js 18+ ve npm
- .NET 9.0 SDK
- PostgreSQL 14+

### 1. Repository'yi Klonlayın

```bash
git clone https://github.com/yourusername/TABUAI.git
cd TABUAI
```

### 2. Backend Kurulumu

```bash
cd tabu-ai/backend/src/TabuAI.API

# appsettings.json dosyasını oluşturun
cp appsettings.example.json appsettings.json

# appsettings.json dosyasını düzenleyin ve şunları güncelleyin:
# - PostgreSQL bağlantı bilgileri
# - Groq API key (https://console.groq.com adresinden alın)

# Veritabanını oluşturun
# PostgreSQL'de tabuai_db veritabanını oluşturun

# Uygulamayı çalıştırın
dotnet run
```

Backend `http://localhost:5092` adresinde çalışacak.

### 3. Frontend Kurulumu

```bash
cd tabu-ai/frontend

# Bağımlılıkları yükleyin
npm install

# Development server'ı başlatın
npm start
```

Frontend `http://localhost:4200` adresinde çalışacak.

## 🎮 Kullanım

1. Tarayıcınızda `http://localhost:4200` adresine gidin
2. **Demo Modu** veya **AI Modu** seçin
3. **Başla** butonuna tıklayın
4. Hedef kelimeyi AI'ya tabu kelimeleri kullanmadan açıklayın
5. AI'nın tahminine ve geri bildirimlerine göz atın
6. Skorunuzu geliştirin!

## ⚙️ Konfigürasyon

### Backend (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=tabuai_db;Username=tabuai_user;Password=your_password"
  },
  "Groq": {
    "ApiKey": "your-groq-api-key-here",
    "BaseUrl": "https://api.groq.com/openai/v1",
    "Model": "llama-3.1-8b-instant"
  }
}
```

### Frontend (environment.ts)

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5092/api'
};
```

### Groq API Key Alma

1. [Groq Console](https://console.groq.com) adresine gidin
2. Hesap oluşturun veya giriş yapın
3. API Keys bölümünden yeni bir key oluşturun
4. Key'i `appsettings.json` dosyasına ekleyin

## 📁 Proje Yapısı

```
TABUAI/
├── tabu-ai/
│   ├── backend/
│   │   └── src/
│   │       ├── TabuAI.API/          # Web API
│   │       ├── TabuAI.Application/  # Business logic
│   │       ├── TabuAI.Domain/       # Domain entities
│   │       └── TabuAI.Infrastructure/ # Data access
│   └── frontend/
│       └── src/
│           ├── app/
│           │   ├── pages/           # Page components
│           │   ├── services/        # API services
│           │   └── models/          # TypeScript models
│           └── environments/        # Environment configs
├── prd.md                           # Product requirements
└── .gitignore                       # Git ignore rules
```

## 🔒 Güvenlik

- ⚠️ **Asla** `appsettings.json` dosyasını git'e commit etmeyin
- ⚠️ **Asla** API key'lerinizi paylaşmayın
- ✅ `.env.example` ve `appsettings.example.json` dosyalarını kullanın
- ✅ Production'da environment variables kullanın

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## 🐛 Sorun Bildirimi

Bir sorun bulduysanız lütfen [Issues](https://github.com/yourusername/TABUAI/issues) sayfasından bildirin.

## 📧 İletişim

Sorularınız için: [your-email@example.com](mailto:your-email@example.com)

---

**Not**: Bu proje eğitim amaçlıdır ve prompt engineering becerilerini geliştirmek için tasarlanmıştır.
