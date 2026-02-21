#!/bin/zsh
#./deploy-ios.sh çalıştırmak için bunu kullan
# TabuAI iOS Deployment Script 🚀

echo "📦 Web projesi derleniyor (ng build)..."
npm run build

if [ $? -ne 0 ]; then
    echo "❌ Derleme hatası! İşlem durduruldu."
    exit 1
fi

echo "🔄 Capacitor Sync yapılıyor (ios)..."
npx cap sync ios

if [ $? -ne 0 ]; then
    echo "❌ Sync hatası! İşlem durduruldu."
    exit 1
fi

echo "📱 Uygulama iOS simülatöründe çalıştırılıyor (iPhone 17 Pro)..."
# Not: Farklı bir simülatör kullanmak isterseniz ID'yi güncelleyebilirsiniz.
npx cap run ios --target 2B699DA9-A7D5-42E0-BF6C-568EBEF58AF3

echo "✅ İşlem tamamlandı!"
