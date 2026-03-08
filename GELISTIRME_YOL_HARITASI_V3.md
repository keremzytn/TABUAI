# 🚀 TabuAI - Gelecek Geliştirme Yol Haritası (V3)

TabuAI projesinin temel özellikleri (Solo Mod, Liderlik Tablosu, Rozet Sistemi) başarıyla tamamlandı. Projeyi bir üst seviyeye taşımak ve kullanıcı etkileşimini artırmak için aşağıdaki geliştirmeler planlanabilir:

## 1. ⚔️ Çok Oyunculu ve Sosyal Etkileşim (Versus Mode)
Kullanıcıların sadece AI'ya karşı değil, birbirlerine karşı da yarıştığı bir yapı:
- **Gerçek Zamanlı Düello:** İki oyuncunun aynı kelimeyi AI'ya en az promptla anlatmaya çalıştığı 1v1 mod (SignalR veya Socket.io entegrasyonu).
- **Arkadaşlarla Kelime Paylaşımı:** Bir kullanıcının kendi seçtiği zor bir kelimeyi arkadaşına "Meydan Okuma" (Challenge) olarak göndermesi.
- **Topluluk Skorları:** Arkadaş listenizdeki kişilerin son oyunlarını ve kazandıkları başarıları görebileceğiniz bir akış (Activity Feed).

## 2. 🤖 İleri Seviye AI Özellikleri
AI etkileşimini daha zengin ve eğlenceli hale getirmek:
- **AI Personaları:** Farklı karakterlere sahip AI'lar (Örn: "Sarkastik AI", "Çocuksu AI", "Aşırı Titiz AI"). Her persona, kelimeyi tahmin ederken farklı tepkiler verir.
- **Sesli Komut (Voice-to-Prompt):** Kullanıcıların promptlarını yazmak yerine mikrofonla söyleyebilmesi (Web Speech API).
- **Prompt Analiz Koçu:** Oyun sonunda AI'nın "Bu kelimeyi şu şekilde anlatsaydın daha etkili olurdu" diyerek prompt mühendisliği dersi vermesi.

## 3. 🌍 Globalizasyon ve İçerik Çeşitliliği
Projeyi dünya çapında bir uygulamaya dönüştürmek:
- **Çoklu Dil Desteği:** İngilizce, Almanca, Fransızca gibi dillerde oyun oynama imkanı. (Prompting becerisi dilden dile nasıl değişiyor?)
- **Kullanıcı Yapımı Kelime Setleri (UGC):** Kullanıcıların kendi tematik paketlerini (Örn: "Harry Potter Evreni", "Yazılım Terimleri") oluşturup toplulukla paylaşması.
- **Günün Kelimesi (Daily Challenge):** Tüm dünyadaki kullanıcıların her gün aynı kelime paketini çözdüğü ve o günün liderinin belirlendiği mod.

## 4. 📈 Gelişmiş İstatistikler ve Profil
Kullanıcının gelişimini somutlaştıran veriler:
- **Prompt Analiz Grafiği:** Kullanıcının zamanla prompt uzunluğu, kelime dağarcığı ve başarı oranındaki değişimi gösteren grafikler.
- **Stil Analizi:** "Sen bir Tanımlama Ustasısın" veya "En çok sıfat kullanan oyuncu" gibi kişiselleştirilmiş analizler.
- **Başarı Galerisi:** Kazanılan nadir rozetlerin sergilenebileceği özelleştirilebilir profil vitrini.

## 5. 💰 Oyun Ekonomisi ve Motivasyon
Kullanıcıyı uygulamada tutacak "Gamification" öğeleri:
- **Tabu Paraları (PromptCoins):** Oyun kazandıkça biriken ve ipucu almak veya profil özelleştirmeleri (avatar, kart tasarımları) için kullanılan sanal para.
- **Seri (Streak) Sistemi:** 5 gün üst üste oynayan kullanıcılara özel çarpanlar veya kozmetik ödüller verilmesi.

## 7. ⚙️ Teknik Altyapı ve Verimlilik
Uygulamanın performansını ve sürdürülebilirliğini artırmak:
- **Hibrit AI Seçimi:** Kullanıcının (veya sistemin) düşük maliyetli (Llama) veya yüksek başarılı (GPT-4) modeller arasında otomatik geçiş yapabilmesi.
- **Caching ve Prompt Optimizasyonu:** Sıkça sorulan benzer promptların sonuçlarının önbelleğe alınarak AI maliyetinin ve yanıt süresinin düşürülmesi (Redis).
- **Offline Mod (Edge AI):** Basit kelimeler için tarayıcı üzerinde çalışan (WebLLM gibi) yerel modellerle internet olmadan da oynayabilme.
- **Kurumsal SSO (Single Sign-On):** Şirket içi eğitimlerde kullanılmak üzere Azure AD veya Google Workspace entegrasyonu.

---
**Not:** Bu geliştirmeler, TabuAI'yı sadece bir oyun olmaktan çıkarıp, eğlenirken **Prompt Engineering** öğreten bir platforma dönüştürecektir.
