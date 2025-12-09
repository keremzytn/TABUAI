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
        <div class="hero-content">
          <h1 class="hero-title">
            Prompt Engineering Oyunu
          </h1>
          <p class="hero-description">
            Yapay zeka ile etkili iletişim kurma becerilerinizi geliştirin. 
            Klasik TABU oyunundan ilham alan bu deneyimle, AI'ya nasıl daha iyi talimatlar vereceğinizi öğrenin.
          </p>
          <div class="hero-actions">
            <a routerLink="/game" class="btn btn-primary btn-large">
              🚀 Oyuna Başla
            </a>
          </div>
        </div>
      </section>

      <!-- Features Section -->
      <section class="features-section">
        <h2 class="section-title">Özellikler</h2>
        
        <div class="features-grid">
          <div class="feature-card">
            <div class="feature-icon">🤖</div>
            <h3>AI Değerlendirme</h3>
            <p>Promptlarınız gerçek zamanlı olarak analiz edilir ve geri bildirim alırsınız.</p>
          </div>

          <div class="feature-card">
            <div class="feature-icon">🎯</div>
            <h3>Hedef Odaklı</h3>
            <p>Her oyunda farklı kelimeler ve zorluk seviyeleri ile becerilerinizi geliştirin.</p>
          </div>

          <div class="feature-card">
            <div class="feature-icon">📈</div>
            <h3>İlerleme Takibi</h3>
            <p>Performansınızı izleyin ve zamanla nasıl geliştiğinizi görün.</p>
          </div>

          <div class="feature-card">
            <div class="feature-icon">🏆</div>
            <h3>Gamification</h3>
            <p>Seviye sistemi ve liderlik tablosu ile motivasyonunuzu yüksek tutun.</p>
          </div>
        </div>
      </section>

      <!-- How It Works Section -->
      <section class="how-section">
        <h2 class="section-title">Nasıl Çalışır?</h2>

        <div class="steps-container">
          <div class="step">
            <div class="step-number">1</div>
            <div class="step-content">
              <h3>Kelime ve Tabu Listesi</h3>
              <p>Size bir hedef kelime ve kullanamayacağınız tabu kelimeler verilir.</p>
            </div>
          </div>

          <div class="step">
            <div class="step-number">2</div>
            <div class="step-content">
              <h3>Prompt Yazın</h3>
              <p>Tabu kelimeleri kullanmadan, AI'ın hedef kelimeyi tahmin edebilmesi için prompt yazın.</p>
            </div>
          </div>

          <div class="step">
            <div class="step-number">3</div>
            <div class="step-content">
              <h3>Sonuç ve Geri Bildirim</h3>
              <p>AI tahminini yapar, performansınızı değerlendirir ve öneriler sunar.</p>
            </div>
          </div>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .hero-section {
      text-align: center;
      padding: 60px 0;
      max-width: 800px;
      margin: 0 auto;
    }

    .hero-title {
      font-size: 3rem;
      font-weight: 700;
      color: #1a202c;
      margin-bottom: 24px;
      line-height: 1.2;
    }

    .hero-description {
      font-size: 1.125rem;
      line-height: 1.7;
      color: #4a5568;
      margin-bottom: 32px;
    }

    .hero-actions {
      display: flex;
      gap: 12px;
      justify-content: center;
    }

    .btn-large {
      padding: 14px 32px;
      font-size: 1rem;
    }

    .features-section {
      margin: 80px 0;
    }

    .section-title {
      font-size: 2rem;
      font-weight: 700;
      text-align: center;
      margin-bottom: 48px;
      color: #1a202c;
    }

    .features-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 24px;
    }

    .feature-card {
      text-align: center;
      padding: 32px 24px;
      background: white;
      border-radius: 12px;
      border: 1px solid #e2e8f0;
      transition: all 0.2s ease;
    }

    .feature-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
      border-color: #cbd5e0;
    }

    .feature-icon {
      font-size: 2.5rem;
      margin-bottom: 16px;
    }

    .feature-card h3 {
      font-size: 1.125rem;
      font-weight: 600;
      margin-bottom: 12px;
      color: #2d3748;
    }

    .feature-card p {
      color: #718096;
      line-height: 1.6;
      font-size: 0.9375rem;
    }

    .how-section {
      margin: 80px 0;
    }

    .steps-container {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 32px;
      margin-top: 48px;
    }

    .step {
      text-align: center;
    }

    .step-number {
      width: 48px;
      height: 48px;
      background: #667eea;
      color: white;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.25rem;
      font-weight: 700;
      margin: 0 auto 20px;
    }

    .step-content h3 {
      font-size: 1.125rem;
      font-weight: 600;
      margin-bottom: 12px;
      color: #2d3748;
    }

    .step-content p {
      color: #718096;
      line-height: 1.6;
      font-size: 0.9375rem;
    }

    @media (max-width: 768px) {
      .hero-title {
        font-size: 2rem;
      }

      .hero-description {
        font-size: 1rem;
      }

      .features-grid {
        grid-template-columns: 1fr;
      }

      .steps-container {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class HomeComponent { }