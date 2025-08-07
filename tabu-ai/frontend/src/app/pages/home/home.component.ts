import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container">
      <!-- Hero Section -->
      <section class="hero-section">
        <div class="hero-content fade-in">
          <h1 class="hero-title">
            🎯 TABU.AI
            <span class="hero-subtitle">Prompt Engineering Oyunu</span>
          </h1>
          <p class="hero-description">
            Yapay zeka ile etkili iletişim kurma becerilerinizi geliştirin! 
            Klasik TABU oyunundan ilham alan bu eğlenceli deneyimle, 
            AI'ya nasıl daha iyi talimatlar vereceğinizi öğrenin.
          </p>
          <div class="hero-actions">
            <a routerLink="/game" class="btn btn-primary btn-large pulse">
              🚀 Oyuna Başla
            </a>
            <a href="#features" class="btn btn-secondary btn-large">
              📖 Nasıl Çalışır?
            </a>
          </div>
        </div>
        <div class="hero-visual">
          <div class="game-preview">
            <div class="preview-card">
              <h3>🎯 Hedef Kelime</h3>
              <div class="target-word">UÇAK</div>
              <div class="tabu-words">
                <span class="tabu-word">hava</span>
                <span class="tabu-word">kanat</span>
                <span class="tabu-word">pilot</span>
                <span class="tabu-word">yolcu</span>
                <span class="tabu-word">uçmak</span>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Features Section -->
      <section id="features" class="features-section">
        <div class="section-header">
          <h2>✨ Neden TABU.AI?</h2>
          <p>Prompt engineering becerilerinizi geliştirmenin en eğlenceli yolu</p>
        </div>
        
        <div class="features-grid">
          <div class="feature-card fade-in">
            <div class="feature-icon">🤖</div>
            <h3>AI Tabanlı Değerlendirme</h3>
            <p>Modern yapay zeka teknolojileri ile promptlarınız gerçek zamanlı olarak analiz edilir ve geri bildirim alırsınız.</p>
          </div>

          <div class="feature-card fade-in">
            <div class="feature-icon">🎯</div>
            <h3>Hedef Odaklı Öğrenme</h3>
            <p>Her oyunda farklı kelimeler ve zorluk seviyeleri ile prompt yazma becerilerinizi sistematik olarak geliştirin.</p>
          </div>

          <div class="feature-card fade-in">
            <div class="feature-icon">📈</div>
            <h3>İlerleme Takibi</h3>
            <p>Performansınızı izleyin, güçlü ve zayıf yönlerinizi keşfedin, zamanla nasıl geliştiğinizi görün.</p>
          </div>

          <div class="feature-card fade-in">
            <div class="feature-icon">🏆</div>
            <h3>Gamification</h3>
            <p>Seviye sistemi, rozetler ve liderlik tablosu ile motivasyonunuzu yüksek tutun ve diğer oyuncularla yarışın.</p>
          </div>

          <div class="feature-card fade-in">
            <div class="feature-icon">💡</div>
            <h3>Akıllı Öneriler</h3>
            <p>AI asistanı her oyun sonrası kişiselleştirilmiş iyileştirme önerileri sunar ve prompt kalitesini artırmanıza yardımcı olur.</p>
          </div>

          <div class="feature-card fade-in">
            <div class="feature-icon">🌟</div>
            <h3>Eğlenceli Deneyim</h3>
            <p>Öğrenmeyi oyuna dönüştüren interaktif arayüz ile prompt engineering hiç bu kadar eğlenceli olmamıştı.</p>
          </div>
        </div>
      </section>

      <!-- How It Works Section -->
      <section class="how-it-works-section">
        <div class="section-header">
          <h2>🎮 Nasıl Çalışır?</h2>
          <p>Sadece 3 adımda prompt engineering öğrenin</p>
        </div>

        <div class="steps-container">
          <div class="step fade-in">
            <div class="step-number">1</div>
            <div class="step-content">
              <h3>📝 Kelime ve Tabu Listesi</h3>
              <p>Size bir hedef kelime ve o kelimeyi anlatırken kullanamayacağınız tabu kelimeler verilir.</p>
            </div>
          </div>

          <div class="step fade-in">
            <div class="step-number">2</div>
            <div class="step-content">
              <h3>✍️ Prompt Yazın</h3>
              <p>Tabu kelimeleri kullanmadan, AI'ın hedef kelimeyi tahmin edebilmesi için açıklayıcı bir prompt yazın.</p>
            </div>
          </div>

          <div class="step fade-in">
            <div class="step-number">3</div>
            <div class="step-content">
              <h3>🎯 Sonuç ve Geri Bildirim</h3>
              <p>AI tahminini yapar, performansınızı değerlendirir ve gelişim için öneriler sunar.</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Call to Action -->
      <section class="cta-section">
        <div class="cta-content fade-in">
          <h2>🚀 Prompt Engineering Yolculuğunuza Başlayın!</h2>
          <p>Yapay zeka ile daha etkili iletişim kurmak için gereken becerileri eğlenceli bir şekilde öğrenin.</p>
          <a routerLink="/game" class="btn btn-primary btn-large pulse">
            🎯 Hemen Oynamaya Başla
          </a>
          <p class="cta-note">Ücretsiz • Kayıt Gerektirmez • Anında Başlayın</p>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .hero-section {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 60px;
      align-items: center;
      min-height: 80vh;
      margin-bottom: 80px;
    }

    .hero-title {
      font-size: 3.5rem;
      font-weight: bold;
      background: linear-gradient(45deg, #667eea, #764ba2);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
      margin-bottom: 16px;
      line-height: 1.2;
    }

    .hero-subtitle {
      display: block;
      font-size: 1.5rem;
      color: #6c757d;
      margin-top: 8px;
    }

    .hero-description {
      font-size: 1.25rem;
      line-height: 1.6;
      color: #495057;
      margin-bottom: 32px;
    }

    .hero-actions {
      display: flex;
      gap: 16px;
      flex-wrap: wrap;
    }

    .btn-large {
      padding: 16px 32px;
      font-size: 1.1rem;
      font-weight: 600;
    }

    .game-preview {
      display: flex;
      justify-content: center;
      align-items: center;
      height: 100%;
    }

    .preview-card {
      background: white;
      border-radius: 20px;
      padding: 32px;
      box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
      text-align: center;
      max-width: 400px;
      transform: rotate(-2deg);
      transition: transform 0.3s ease;
    }

    .preview-card:hover {
      transform: rotate(0deg) scale(1.05);
    }

    .preview-card h3 {
      margin-bottom: 16px;
      color: #333;
      font-size: 1.2rem;
    }

    .target-word {
      font-size: 2.5rem;
      font-weight: bold;
      color: #667eea;
      margin: 20px 0;
      padding: 16px;
      background: rgba(102, 126, 234, 0.1);
      border-radius: 12px;
    }

    .tabu-words {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      justify-content: center;
      margin-top: 20px;
    }

    .features-section {
      margin: 100px 0;
    }

    .section-header {
      text-align: center;
      margin-bottom: 60px;
    }

    .section-header h2 {
      font-size: 2.5rem;
      margin-bottom: 16px;
      color: #333;
    }

    .section-header p {
      font-size: 1.2rem;
      color: #6c757d;
    }

    .features-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 32px;
    }

    .feature-card {
      text-align: center;
      padding: 32px 24px;
      background: white;
      border-radius: 16px;
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
      transition: transform 0.3s ease, box-shadow 0.3s ease;
    }

    .feature-card:hover {
      transform: translateY(-8px);
      box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15);
    }

    .feature-icon {
      font-size: 3rem;
      margin-bottom: 16px;
    }

    .feature-card h3 {
      font-size: 1.25rem;
      margin-bottom: 16px;
      color: #333;
    }

    .feature-card p {
      color: #6c757d;
      line-height: 1.6;
    }

    .how-it-works-section {
      margin: 100px 0;
      background: rgba(255, 255, 255, 0.5);
      padding: 60px 0;
      border-radius: 30px;
    }

    .steps-container {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 40px;
      margin-top: 60px;
    }

    .step {
      text-align: center;
      position: relative;
    }

    .step-number {
      width: 60px;
      height: 60px;
      background: linear-gradient(45deg, #667eea, #764ba2);
      color: white;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.5rem;
      font-weight: bold;
      margin: 0 auto 24px;
      box-shadow: 0 8px 20px rgba(102, 126, 234, 0.3);
    }

    .step-content h3 {
      font-size: 1.25rem;
      margin-bottom: 16px;
      color: #333;
    }

    .step-content p {
      color: #6c757d;
      line-height: 1.6;
    }

    .cta-section {
      margin: 100px 0;
      text-align: center;
      padding: 60px 0;
      background: linear-gradient(135deg, rgba(102, 126, 234, 0.1), rgba(118, 75, 162, 0.1));
      border-radius: 30px;
    }

    .cta-content h2 {
      font-size: 2.5rem;
      margin-bottom: 16px;
      color: #333;
    }

    .cta-content p {
      font-size: 1.2rem;
      color: #6c757d;
      margin-bottom: 32px;
    }

    .cta-note {
      font-size: 0.9rem;
      color: #6c757d;
      margin-top: 16px;
    }

    @media (max-width: 768px) {
      .hero-section {
        grid-template-columns: 1fr;
        gap: 40px;
        text-align: center;
      }

      .hero-title {
        font-size: 2.5rem;
      }

      .hero-actions {
        justify-content: center;
      }

      .preview-card {
        transform: none;
      }

      .steps-container {
        grid-template-columns: 1fr;
      }

      .features-grid {
        grid-template-columns: 1fr;
      }

      .cta-content h2 {
        font-size: 2rem;
      }
    }
  `]
})
export class HomeComponent {}